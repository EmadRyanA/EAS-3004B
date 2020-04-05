using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DSPLib;
using UnityEngine;


/* The class that performs beat map generation given an audioClip
 * Sources used during development:
 * https://medium.com/giant-scam/algorithmic-beat-mapping-in-unity-preprocessed-audio-analysis-d41c339c135a for some unity specific steps (e.g running as thread, specifics about the AudioClip, etc)
 * https://www.codeproject.com/Articles/1107480/DSPLib-FFT-DFT-Fourier-Transform-Library-for-NET-6 This is the FFT library that we currently use.
 * https://www.badlogicgames.com/wordpress/?p=122 Used to learn about beat detection logic. (this is where concepts like spectral flux, threshold come from)
 * 
 * Author: Evan Jesty (101078735)
 */

public class BGA
{

    public enum STATE
    {
        READY,
        ACTIVE,
        THREAD_ACTIVE,
        ERROR,
        DONE
    }

    public STATE state;

    public const Int32 RANDOM_SEED_LENGTH = 8;

    public const float delta = 0.001f;

    public bga_settings settings;

    public System.Random bga_random;

    //Information Structs

    public song_info_struct song_info;
    public song_meta_struct song_meta;

    public static string persistentDataPath;

    public string songFilePath;

    //todo: this is a lot of ram! get rid of arrays when we are done with them
    public struct output_struct
    {
        public float[][] fftData;
        public float[] flux;
        public float[] flux2;
        public float[] peaks; //true final output
        public float[] peaks2; //peaks filterd to min_time_between_peaks
        public float[] threshold;
        public float[] peakThreshold;
        public float[] drifts;

        public int totalPeaks;
        public float totalPeaksOverTime;

        public int flySectionIndex;
    }

    public struct GreatestValueElement {
          public int index;
          public float value;

          public GreatestValueElement(int index, float value) {
              this.index = index;
              this.value = value;
          }
    };

    public output_struct output;

    public void StartBGA(ref AudioClip audioClip, bga_settings settings, song_meta_struct song, string songFilePath)
    {
        this.songFilePath = songFilePath;
        this.song_meta = song;
        if (this.state != STATE.READY)
        {
            throw new Exception("Cannot start the beat generating algorithim if it is already being run! State: " + this.state);
        }
        this.state = STATE.ACTIVE;
        Debug.Log("Threshold, multiplier:");
        Debug.Log(settings.threshold_time);
        Debug.Log(settings.threshold_multiplier);
        this.settings = settings;
        if (settings.rng_seed == 0)
        {
            this.settings.rng_seed = generateNewRandomSeed();
            Debug.Log(this.settings.rng_seed);
        }
        
        bga_random = new System.Random(settings.rng_seed);

        song_info = new song_info_struct(audioClip);
        song_info.samples = new float[song_info.sampleCount * song_info.channels];
        song_info.sampleLength = song_info.length / (float)song_info.sampleCount;
        //GetData returns samples for both the L(eft) and R(ight) channels
        audioClip.GetData(song_info.samples, 0);

        output = new output_struct();

        persistentDataPath = Application.persistentDataPath; //We can't use unity specific calls in the bga thread, and we need this variable for later

        //Create the background thread and run the BGA algorithim
        //The algorthim will have access to all the public structs
        Thread BGAThread = new Thread(new ThreadStart(algorithimWrapper));
        BGAThread.Start();

    }

    public int generateNewRandomSeed()
    {
        return Mathf.FloorToInt(new System.Random().Next((int)Mathf.Pow(10, (RANDOM_SEED_LENGTH - 1)), (int)Mathf.Pow(10, RANDOM_SEED_LENGTH) - 1));
    }

    //Background thread to run the algorithim
    void algorithimWrapper()
    {
        this.state = STATE.THREAD_ACTIVE;
        //we need to wrap this in a try catch to output errors properly since this is running on a different thread then the main unity logic
        try
        {
            algorithim();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            this.state = STATE.ERROR;
        }
    }

    //Given a songs samples, total sample count and channel count, combine L and R channels into one mono 'channel'
    //E.g samples = float[]{L1, R1, L2, R2, L3, R3, ...}; the function will return float[]{(L1+R1)/2, (L2+R2)/2, (L3+R3)/2, ...} of length songSampleCount
    float[] getCombinedSamples (ref float[] samples, int songSampleCount, int channels)
    {
        if (channels == 1)
        {
            return samples;
        } else if (channels == 2)
        {
            float[] newSamples = new float[songSampleCount];
            for (int i = 0; i < songSampleCount - 1; i += 1)
            {
                //Mono sample = L + R / 2
                newSamples[i] = (samples[(i * 2)] + samples[(i * 2) + 1]) / 2f;
            }
            return newSamples;
        } else //todo support more than 1 channel (does this ever happen?)
        {
            throw new Exception("# of channels is not in the supported range! (expected 1 or 2, got: " + channels); //todo custom exception   
        }
    }

    //The B(eat) G(enerating) A(lgorithim)
    //Background thread to create the beatmap
    void algorithim ()
    {
        float[] samples = getCombinedSamples(ref song_info.samples, song_info.sampleCount, song_info.channels);

        Debug.Log("Song samples coverted to mono");

        int finalArraySize = song_info.sampleCount / settings.n_bins;

        Debug.Log(finalArraySize);
        output.fftData = new float[finalArraySize][];
        output.flux = new float[finalArraySize];
        output.flux2 = new float[finalArraySize];
        output.peaks = new float[finalArraySize];
        output.peaks2 = new float[finalArraySize];
        output.threshold = new float[finalArraySize];
        output.peakThreshold = new float[finalArraySize];
        output.drifts = new float[finalArraySize];
        output.flySectionIndex = 0;
        FFTProvider fftProvider = new DSPLibFFTProvider(settings.n_bins);
        WINDOW_TYPE fftWindow = WINDOW_TYPE.Hamming;

        //perform fft using N_BINS at a time
        for (int i = 0; i < finalArraySize; i++)
        {
            float[] currSamples = new float[settings.n_bins];
            int startIndex = (i * settings.n_bins);
            //Grab the current sample (length N_BINS) from the samples data
            for (int j = startIndex; j < startIndex + settings.n_bins; j++)
            {
                currSamples[j - startIndex] = samples[j];
            }

            output.fftData[i] = fftProvider.doFFT(currSamples, fftWindow);

            if (i != 0)
            {
                //Compute the spectral flux for the current spectrum
                float flux = 0;
                for (int j = 0; j < output.fftData[i].Length; j++)
                {
                    float currFlux = (output.fftData[i][j] - output.fftData[i - 1][j]);
                    //we only want 'rising' spectral flux - since we are detecting beat onset therefore we only care about rise in power
                    if (currFlux > 0) flux += currFlux;
                }
                output.flux[i] = flux;
            }
            else output.flux[0] = 0;

        }

        //define a window size for the threshold. THRESHOLD_TIME is the length in time that the window should be.
        int thresholdWindowSize = Mathf.FloorToInt((settings.threshold_time / song_info.sampleLength) / settings.n_bins);
        Debug.Log("threshold: ");
        Debug.Log(thresholdWindowSize);

        //Compute threshold for each flux value
        for (int i = 0; i < output.flux.Length; i++)
        {
            float avg = 0;
            float count = 0;
            for (int j = (i - thresholdWindowSize); j < (i + thresholdWindowSize); j++)
            {
                if (j < 0 || j >= output.flux.Length) continue; //todo should be optimized
                avg += output.flux[j];
                count += 1;
            }
            if (count > 0)
            {
                output.threshold[i] = (avg / count) * settings.threshold_multiplier;
            }
            else
            {
                output.threshold[i] = 0f;
            }
        }

        //using the computed threshold, discard any flux values that are below/at the threshold (most likely not a beat as it is below avg)
        for (int i = 0; i < output.flux.Length; i++)
        {
            if (output.flux[i] <= output.threshold[i])
            {
                output.flux2[i] = 0f;
            }
            else
            {
                output.flux2[i] = output.flux[i]; //subtract avg so we see only the peak
            }
        }        

        //Check for peaks: If curr value > next value this is a peak

        for (int i = 0; i < output.flux2.Length - 1; i++)
        {
            if (output.flux2[i] > output.flux2[i + 1])
            {
                output.peaks[i] = output.flux2[i]; //Beat Detected
                output.totalPeaks += 1;
            }
            else
            {
                output.peaks[i] = 0f;
            }
        }

        //avg # of beats per second... multiply by 60 to get bpm
        output.totalPeaksOverTime = output.totalPeaks / song_info.length;

        Debug.Log("BPM: ");
        Debug.Log(output.totalPeaksOverTime * 60);

        //fly detection

        //define a window size for the threshold. THRESHOLD_TIME is the length in time that the window should be. for now use drift threshold time
        int flyThresholdWindowSize = Mathf.FloorToInt((settings.fly_threshold_time / song_info.sampleLength) / settings.n_bins) / 2;
        Debug.Log("threshold3: ");
        Debug.Log(flyThresholdWindowSize);

        //only look at the song between 30% and 70% duration,
        //and only find largest value.

        float largestDelta = 0;
        int indexLargest = 0;

        Debug.Log("Detect fly");
        Debug.Log(Mathf.FloorToInt(.30f * output.peaks.Length));
        Debug.Log(Mathf.FloorToInt(.70f * output.peaks.Length));

        for (int i = Mathf.FloorToInt(.30f * output.peaks.Length); i < Mathf.FloorToInt(.70f * output.peaks.Length); i++)
        {
            float avgL = 0;
            float avgR = 0;
            float countL = 0;
            float countR = 0;

            for (int j = (i - flyThresholdWindowSize); j < (i + flyThresholdWindowSize); j++)
            {
                if (j < 0 || j >= output.peaks.Length) continue; //todo should be optimized
                if (j < i) {
                    avgL += output.peaks[j];
                    countL += 1;
                }
                else {
                    avgR += output.peaks[j];
                    countR += 1;
                }
            }
            if (countL > 0 && countR > 0)
            {
                float avg = (avgL / countL) - (avgR / countR); //we are looking for the biggest difference from left to right
                if (avg > largestDelta) {
                    largestDelta = avg;
                    indexLargest = i;
                }
            }
        }

        output.flySectionIndex = indexLargest;

        Debug.Log("Fly section: ");
        Debug.Log(output.flySectionIndex);

        //end fly detection

        //drift detection

        //define a window size for the threshold. THRESHOLD_TIME is the length in time that the window should be.
        int driftThresholdWindowSize = Mathf.FloorToInt((settings.drift_threshold_time / song_info.sampleLength) / settings.n_bins) / 2;
        Debug.Log("threshold2: ");
        Debug.Log(driftThresholdWindowSize);

        //Compute a threshold on avg # of peaks at a given time
        for (int i = 0; i < output.peaks.Length; i++)
        {
            float avg = 0;
            float count = 0;
            for (int j = (i - driftThresholdWindowSize); j < (i + driftThresholdWindowSize); j++)
            {
                if (j < 0 || j >= output.peaks.Length) continue; //todo should be optimized
                avg += output.peaks[j];
                count += 1;
            }
            if (count > 0)
            {
                output.peakThreshold[i] = (avg / count);
            }
            else
            {
                output.peakThreshold[i] = 0f;
            }
        }

        //select amount_drift_sections from output.peakThreshold, ensuring that each drift section is min_time_between_drift apart
        //remove all duplicates / close values next to a value, then store remaning values as a GreatestValueElement in a arraylist, then sort the list.
        
        List<GreatestValueElement> greatestValues = new List<GreatestValueElement>();

        Debug.Log("Start greatestvalues");
        int offset = 1;
        for (int i = 0; i < output.peakThreshold.Length; i+= offset) {
          offset = 1;
          if ((i - driftThresholdWindowSize) <= 0 || (i + driftThresholdWindowSize) >= output.peakThreshold.Length) { //throw out values near beginning and near end
              continue;
          }
          while (((i + offset) < output.peakThreshold.Length) && Math.Abs(output.peakThreshold[i] - output.peakThreshold[i + offset]) <= delta) {
              output.peakThreshold[i + offset] = 0;
              offset += 1;
          }
          GreatestValueElement elem = new GreatestValueElement(i, output.peakThreshold[i]);
          greatestValues.Add(elem); 
        }

        Debug.Log("Sort greatestvalues");

        //todo debug this...
        try {
            greatestValues.Sort((x, y) => y.value.CompareTo(x.value));
        }
        catch (Exception e) {
            Debug.Log("Could not sort greatestValues!");
            Debug.Log(e.ToString());
        }

        Debug.Log("Song length:");
        Debug.Log(song_info.length);

        int minLengthBetweenDrift = Mathf.FloorToInt((settings.min_drift_seperation_time / song_info.sampleLength) / settings.n_bins);
        int totalAllowableDrifts = Mathf.FloorToInt(settings.drifts_per_minute * Mathf.FloorToInt(song_info.length / 60f));
        int warmUpTimeLength = Mathf.FloorToInt((settings.warm_up_time / song_info.sampleLength) / settings.n_bins);

        Debug.Log("drifts per minute:");
        Debug.Log(settings.drifts_per_minute);

        Debug.Log("Drift sections: ");
        Debug.Log(totalAllowableDrifts);

        Debug.Log("Min length between drifts: ");
        Debug.Log(minLengthBetweenDrift);

        //select elements and make sure they are min_time_between_drift apart
        int numSelected = 0;
        List<GreatestValueElement> addedValues = new List<GreatestValueElement>();
        bool first = true;
        foreach (GreatestValueElement elem in greatestValues) {
            if (numSelected >= totalAllowableDrifts) break;
            if (first) {
                int offSetIndex = Math.Max(elem.index - driftThresholdWindowSize, 0);
                if (offSetIndex == 0) continue;
                if (offSetIndex - warmUpTimeLength <= 0) continue;
                output.drifts[offSetIndex] = elem.value;
                numSelected = 1;
                first = false;
                addedValues.Add(elem);
            }
            else {
                bool flag = false;
                //We need to check if this elem is properly spaced at least min_time_between_drift apart from other drifts
                //And that the elem is not in the middle of a <fly> section or <time_after_fly> section
                foreach (GreatestValueElement added in addedValues) {
                    if (Math.Abs(elem.index - added.index) <= minLengthBetweenDrift
                    || Math.Abs(elem.index - output.flySectionIndex) <= (flyThresholdWindowSize * 2.2)) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                  int offSetIndex = Math.Max(elem.index - driftThresholdWindowSize, 0);
                  if (offSetIndex == 0) continue;
                  if (offSetIndex - warmUpTimeLength <= 0) continue;
                  output.drifts[offSetIndex] = elem.value;
                  numSelected += 1;
                  addedValues.Add(elem);
                }
            }
        }

        //end drift detection
        
        //Filter peaks to allowable min_time_between_peaks
        //Todo this is a bit naive should probably select highest peak or something (but will work for now)
        //int minLengthBetweenPeaks = Mathf.FloorToInt(Mathf.FloorToInt((settings.min_peak_seperation_time / sampleLength) / settings.n_bins) / 2);
        if (settings.min_peak_seperation_time > 0) {
            int minLengthBetweenPeaks = Mathf.FloorToInt((settings.min_peak_seperation_time / song_info.sampleLength) / settings.n_bins);
            for (int i = 0; i < output.peaks.Length; i++)
            {
                if (output.peaks[i] > 0)
                {
                    output.peaks2[i] = output.peaks[i];
                    i += minLengthBetweenPeaks - 1;
                }
                
            }
        }
        else {
            for (int i = 0; i < output.peaks.Length; i++) {
                output.peaks2[i] = output.peaks[i];
            }
        }

        BeatMap beatMap = makeBeatMap(driftThresholdWindowSize * 2, flyThresholdWindowSize * 2);
        beatMap.save(persistentDataPath);

        //todo deal with random stuff below
        //debug output

        Debug.Log("BGA Done");
        this.state = STATE.DONE;

        // using (StreamWriter file = new StreamWriter("output.txt"))
        // {
        //     for (int i=0; i < output.flux.Length; i++)
        //     {
        //         file.WriteLine(output.flux[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output2.txt"))
        // {
        //     for (int i = 0; i < output.threshold.Length; i++)
        //     {
        //         file.WriteLine(output.threshold[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output3.txt"))
        // {
        //     for (int i = 0; i < output.flux2.Length; i++)
        //     {
        //         file.WriteLine(output.flux2[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output4.txt"))
        // {
        //     for (int i = 0; i < output.peaks.Length; i++)
        //     {
        //         file.WriteLine(output.peaks[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output5.txt"))
        // {
        //     for (int i = 0; i < output.peaks2.Length; i++)
        //     {
        //         file.WriteLine(output.peaks2[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output6.txt"))
        // {
        //     for (int i = 0; i < output.peakThreshold.Length; i++)
        //     {
        //         file.WriteLine(output.peakThreshold[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output7.txt"))
        // {
        //     for (int i = 0; i < output.drifts.Length; i++)
        //     {
        //         file.WriteLine(output.drifts[i]);
        //     }
        // }
        // using (StreamWriter file = new StreamWriter("output8.txt"))
        // {
        //     Queue<LaneObject> laneObjects = beatMap.initLaneObjectQueue();
        //     foreach (LaneObject l in laneObjects) {
        //         file.WriteLine(l.sampleIndex + " " + l.lane + " " + l.time + " " + (l.type == LANE_OBJECT_TYPE.Beat ? "1" : "0"));
        //     }
        // }
        

        // Debug.Log("Output file saved");
        

        //frameScale = (int) (songLength / finalArraySize);
        //float sampleLength = song_info.length / (float)song_info.sampleCount;
        //Debug.Log(sampleLength);

        //Debug.Log(sampleLength * 1000);
        //Debug.Log(sampleLength * N_BINS); //length per reading

        //done = true;

    }

    float getTimeFromIndex(int index) {
        return song_info.sampleLength * index * settings.n_bins;
    }
    float getIndexFromTime(float time) {
        return (Mathf.FloorToInt(time / song_info.sampleLength)) / settings.n_bins;
    }

    //All peaks are detected - now its time to decide where these beats are going
    BeatMap makeBeatMap(int drift_length, int fly_length)
    {
        BeatMap beatMap = new BeatMap(settings, song_info, song_meta, songFilePath);

        int currLane = 0;
        int currLaneCount = 0;

        int timeAfterFly = Mathf.FloorToInt(getIndexFromTime(BGACommon.TIME_AFTER_FLY_SECTION));

        int endFlyIndex = -1;
        
        for (int i=0; i < output.peaks2.Length; i++) {
            float currTime = getTimeFromIndex(i);
            if (i == output.flySectionIndex) {
              LaneObject lObj1 = new LaneObject(i, currTime, -1, LANE_OBJECT_TYPE.START_FLY_TRIGGER);
              LaneObject lObj2 = new LaneObject(i + fly_length, getTimeFromIndex(i + fly_length), -1, LANE_OBJECT_TYPE.START_NORMAL_TRIGGER);
              endFlyIndex = i + drift_length;
              beatMap.addLaneObject(lObj1);
              beatMap.addLaneObject(lObj2);
              Debug.Log("Added the fly section triggers");
              continue;
            }
            else if (output.drifts[i] > 0) {
              LaneObject lObj1 = new LaneObject(i, currTime, -1, LANE_OBJECT_TYPE.START_DRIFT_TRIGGER);
              LaneObject lObj2 = new LaneObject(i + drift_length, getTimeFromIndex(i + drift_length), -1, LANE_OBJECT_TYPE.START_NORMAL_TRIGGER);
              beatMap.addLaneObject(lObj1);
              beatMap.addLaneObject(lObj2);
              continue;
            }
            else if (i == endFlyIndex) { //Don't spawn anything after a <fly> section so that the car can fall back to the track
                endFlyIndex = -1;
                i += timeAfterFly;
                continue;
            }
            else if (output.peaks2[i] <= 0) {
                continue;
            } 
            else if (currTime < settings.warm_up_time) { //we do not spawn notes until warm up time is done. So we spawn obstacles on L and R to show the beats
                int lane = (bga_random.Next(0, 2) == 1) ? 0 : 2;
                LaneObject lObj = new LaneObject(i, currTime, lane, LANE_OBJECT_TYPE.Obstacle);
                beatMap.addLaneObject(lObj);
            }
            else {
                //we have not spawned a beat yet, choose a random lane
                if (currLaneCount == 0) {
                    currLane = bga_random.Next(0, 3);
                    currLaneCount = 1;
                }
                else {
                    if (bga_random.NextDouble() < BGACommon.CHANCE_TO_SWITCH_LANES) { //5 % chance we stay in the same lane
                        currLaneCount = 1;
                        int direction = (bga_random.Next(0, 2) == 1) ? -1 : 1; //move left or right? if on ends wrap around
                        currLane = Math.Abs((currLane + direction) % BGACommon.NUMBER_LANES);
                    }
                    else {
                        currLaneCount += 1;
                    }
                    int randomObstacle = bga_random.Next(0, 4);
                    switch (randomObstacle) {
                        case 0: break;
                        case 1:
                            //add an obstacle on lane n + 1
                            beatMap.addLaneObject(new LaneObject(i, currTime, Math.Abs((currLane + 1) % BGACommon.NUMBER_LANES), LANE_OBJECT_TYPE.Obstacle));
                            break;
                        
                        case 2:
                            //add an obstacle on lane n - 1
                            beatMap.addLaneObject(new LaneObject(i, currTime, Math.Abs((currLane - 1) % BGACommon.NUMBER_LANES), LANE_OBJECT_TYPE.Obstacle));
                            break;
                        
                        case 3:
                            //add an obstacle on lanes n - 1 and n + 1
                            beatMap.addLaneObject(new LaneObject(i, currTime, Math.Abs((currLane + 1) % BGACommon.NUMBER_LANES), LANE_OBJECT_TYPE.Obstacle));
                            beatMap.addLaneObject(new LaneObject(i, currTime, Math.Abs((currLane - 1) % BGACommon.NUMBER_LANES), LANE_OBJECT_TYPE.Obstacle));
                            break;
                        default:
                            throw new Exception("randomObstacle is an unexpected value! Expected [0, 3]; actual: " + randomObstacle);
                        
                    }
                    
                    LaneObject lObj = new LaneObject(i, currTime, currLane, LANE_OBJECT_TYPE.Beat);
                    beatMap.addLaneObject(lObj);

                }
            }
        }

        return beatMap;
    }
}