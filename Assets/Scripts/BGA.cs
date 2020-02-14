using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DSPLib;
using UnityEngine;

/* The class that performs beat map generation given an audioClip
 * Sources used during development:
 * https://medium.com/giant-scam/algorithmic-beat-mapping-in-unity-preprocessed-audio-analysis-d41c339c135a for unity specific steps (e.g getting FFT data for all of the sample length)
 * https://www.codeproject.com/Articles/1107480/DSPLib-FFT-DFT-Fourier-Transform-Library-for-NET-6 This is the FFT library that we currently use.
 * https://www.badlogicgames.com/wordpress/?p=122 Lots of usefull information from this one... Mostly used for beat detection (after unity specific steps are applied)
 */

public class BGA : MonoBehaviour
{

    public const int N_BINS = 1024; //todo make these options
    public const float THRESHOLD_TIME = 0.5f;
    public const float THRESHOLD_MULTIPLIER = 1.5f;

    public struct song_info_struct
    {
        public AudioSource audioSource;
        public AudioClip audioClip;
        public float length;
        public int frequency;
        public int sampleCount;
        public int channels;
        public float[] samples;
    }

    public song_info_struct song_info;

    //todo beatmap
    public struct output_struct
    {
        public float[][] fftData;
        public float[] flux;
        public float[] flux2;
        public float[] peaks; //true final output
        public float[] threshold;
    }

    public output_struct output;

    //todo gamestate needs to be moved
    public bool done = false;
    bool doPlay = false;
    public int frameCount = 0;

    //Allow to enable/disable while testing
    public bool enabled = false;
 
    // Start is called before the first frame update
    void Start()
    {
        if (!enabled) return;
        //todo only using audiosource to get the audioclip
        song_info.audioSource = GetComponent<AudioSource>();
        song_info.audioClip = song_info.audioSource.clip;

        //Unity's internal sample rate is 2* what we need for our calculations
        //float sampleRate = AudioSettings.outputSampleRate / 2f;
        //float binFrequency = sampleRate / N_BINS; //Compute how much frequency each bin contains

        //If we were doing real time we could use this function
        //float[] spectrum = new float[N_BINS];
        //audioSource.GetSpectrumData(spectrum, 0, FFT_WINDOW);
        //Instead we use a background thread to process all samples from audioClip

        song_info.sampleCount = song_info.audioClip.samples;
        song_info.channels = song_info.audioClip.channels;
        song_info.length = song_info.audioClip.length;
        Debug.Log(song_info.length);
        //GetData returns samples for both the L(eft) and R(ight) channels
        song_info.samples = new float[song_info.sampleCount * song_info.channels];
        song_info.audioClip.GetData(song_info.samples, 0);

        //Create the background thread and run the BGA algorithim
        //The algorthim will have acess to the song_info struct
        Thread BGAThread = new Thread(new ThreadStart(algorithimWrapper));
        BGAThread.Start();

    }

    //Background thread to run the algorithim
    void algorithimWrapper()
    {
        //we need to wrap this in a try catch to output errors properly since this is running on a different thread then the main unity logic
        try
        {
            algorithim();
        }
        catch (Exception e)
        {
            Debug.Log(e);
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
    //Background  to create the beatmap
    void algorithim ()
    {
        float[] samples = getCombinedSamples(ref song_info.samples, song_info.sampleCount, song_info.channels);

        Debug.Log("Song samples coverted to mono");

        int finalArraySize = song_info.sampleCount / N_BINS;

        Debug.Log(finalArraySize);
        output.fftData = new float[finalArraySize][];
        output.flux = new float[finalArraySize];
        output.flux2 = new float[finalArraySize];
        output.peaks = new float[finalArraySize];
        output.threshold = new float[finalArraySize];
        FFTProvider fftProvider = new DSPLibFFTProvider(N_BINS);
        WINDOW_TYPE fftWindow = WINDOW_TYPE.Hamming;

        //perform N_BINS of fft at a time
        for (int i = 0; i < finalArraySize; i++)
        {
            float[] currSamples = new float[N_BINS];
            int startIndex = (i * N_BINS);
            //Grab the current sample (length N_BINS) from the samples data
            for (int j = startIndex; j < startIndex + N_BINS; j++)
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

        float sampleLength = song_info.length / (float)song_info.sampleCount;

        //define a window size for the threshold. THRESHOLD_TIME is the length in time that the window should be.
        int thresholdWindowSize = Mathf.FloorToInt(Mathf.FloorToInt((THRESHOLD_TIME / sampleLength) / N_BINS) / 2);
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
                output.threshold[i] = (avg / count) * THRESHOLD_MULTIPLIER;
            }
            else
            {
                output.threshold[i] = 0f;
            }
        }

        //using the computed threshold, discard any flux values that are below/at the threshold (100% not a beat as it is below avg)
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
                output.peaks[i] = output.flux2[i]; //Beat detected todo
            }
            else
            {
                output.peaks[i] = 0f;
            }
        }

        //todo deal with random stuff below

        //debug output

        Debug.Log("FFT Data collected");

        using (StreamWriter file = new StreamWriter("output.txt"))
        {
            /*
            for (int i=0; i<output.fftData.Length; i++)
            {
                StringBuilder builder = new StringBuilder();
                for (int j=0;j<output.fftData[i].Length;j++)
                {
                    builder.AppendFormat("{0}, ", output.fftData[i][j].ToString());
                }
                file.WriteLine(builder.ToString());
            }
            */
            for (int i=0; i < output.flux.Length; i++)
            {
                file.WriteLine(output.flux[i]);
            }
        }
        using (StreamWriter file = new StreamWriter("output2.txt"))
        {
            for (int i = 0; i < output.threshold.Length; i++)
            {
                file.WriteLine(output.threshold[i]);
            }
        }
        using (StreamWriter file = new StreamWriter("output3.txt"))
        {
            for (int i = 0; i < output.flux2.Length; i++)
            {
                file.WriteLine(output.flux2[i]);
            }
        }
        using (StreamWriter file = new StreamWriter("output4.txt"))
        {
            for (int i = 0; i < output.peaks.Length; i++)
            {
                file.WriteLine(output.peaks[i]);
            }
        }

        Debug.Log("Output file saved");

        //frameScale = (int) (songLength / finalArraySize);
        //float sampleLength = song_info.length / (float)song_info.sampleCount;
        Debug.Log(sampleLength);

        //Debug.Log(sampleLength * 1000);
        Debug.Log(sampleLength * N_BINS); //length per reading

        done = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            if (!doPlay)
            {
                song_info.audioSource.Play();
                doPlay = true;
            }
            frameCount += 1;
            float sampleLength = song_info.length / (float)song_info.sampleCount;
            int location = (Mathf.FloorToInt(song_info.audioSource.time / sampleLength)) / N_BINS;
            float[] spectrum = output.fftData[location];
            for (int i = 1; i < spectrum.Length - 1; i++)
            {
                Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
                Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
                Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
                Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
            }
        }

    }
}
