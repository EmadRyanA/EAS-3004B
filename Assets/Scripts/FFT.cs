using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DSPLib;

/*  Define an abstract class that performs the FFT algorthim along with any needed FFT window functions
 */

//Define generic names for FFT window types, implementations must parse this and convert to an internal representation of the window type
public enum WINDOW_TYPE
{
    Hamming,
    None
}

public abstract class FFTProvider
{
    //Do fft on the samples with the given window
    //# of bins == samples.Length
    public abstract float[] doFFT(float[] samples, WINDOW_TYPE window);
}

/* The class that performs FFT using the DSPLib
 * Requires https://www.codeproject.com/Articles/1107480/DSPLib-FFT-DFT-Fourier-Transform-Library-for-NET-6
 * Requires System.Numerics.Complex
 */
public class DSPLibFFTProvider : FFTProvider
{
    FFT fft;
    int binSize;
    
    public DSP.Window.Type getWindowType(WINDOW_TYPE type)
    {
        switch (type)
        {
            case WINDOW_TYPE.Hamming: return DSP.Window.Type.Hamming;
            default: return DSP.Window.Type.Rectangular;
        }
    }

    public DSPLibFFTProvider (int binSize)
    {
        fft = new FFT();
        fft.Initialize((UInt32)binSize);
        this.binSize = binSize;
    }

    public override float[] doFFT(float[] samples, WINDOW_TYPE window)
    {
        double[] dSamples = new double[samples.Length];
        for (int i=0; i< samples.Length; i++)
        {
            dSamples[i] = (double)samples[i];
        }

        //Code from DSPLib documentation
        DSP.Window.Type dspWindow = getWindowType(window);
        double[] windowCoefs = DSP.Window.Coefficients(dspWindow, (UInt32)this.binSize);
        double[] windowInputData = DSP.Math.Multiply(dSamples, windowCoefs);
        double windowScaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

        System.Numerics.Complex[] complexSpectrum = fft.Execute(windowInputData);
        //Convert to magnitude and multiply by the window scale factor to get our binned array
        double[] fftSpectrum = DSP.Math.Multiply(DSPLib.DSP.ConvertComplex.ToMagnitude(complexSpectrum), windowScaleFactor);
        float[] fFFTSpectrum = new float[fftSpectrum.Length];
        for (int i=0; i < fFFTSpectrum.Length; i++)
        {
            fFFTSpectrum[i] = (float)fftSpectrum[i];
        }
        return fFFTSpectrum;
    }
}
=======
﻿using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DSPLib;

/*  Define an abstract class that performs the FFT algorthim along with any needed FFT window functions
 */

//Define generic names for FFT window types, implementations must parse this and convert to an internal representation of the window type
public enum WINDOW_TYPE
{
    Hamming,
    None
}

public abstract class FFTProvider
{
    //Do fft on the samples with the given window
    //# of bins == samples.Length
    public abstract float[] doFFT(float[] samples, WINDOW_TYPE window);
}

/* The class that performs FFT using the DSPLib
 * Requires https://www.codeproject.com/Articles/1107480/DSPLib-FFT-DFT-Fourier-Transform-Library-for-NET-6
 * Requires System.Numerics.Complex
 */
public class DSPLibFFTProvider : FFTProvider
{
    FFT fft;
    int binSize;
    
    public DSP.Window.Type getWindowType(WINDOW_TYPE type)
    {
        switch (type)
        {
            case WINDOW_TYPE.Hamming: return DSP.Window.Type.Hamming;
            default: return DSP.Window.Type.Rectangular;
        }
    }

    public DSPLibFFTProvider (int binSize)
    {
        fft = new FFT();
        fft.Initialize((UInt32)binSize);
        this.binSize = binSize;
    }

    public override float[] doFFT(float[] samples, WINDOW_TYPE window)
    {
        double[] dSamples = new double[samples.Length];
        for (int i=0; i< samples.Length; i++)
        {
            dSamples[i] = (double)samples[i];
        }

        //Code from DSPLib documentation
        DSP.Window.Type dspWindow = getWindowType(window);
        double[] windowCoefs = DSP.Window.Coefficients(dspWindow, (UInt32)this.binSize);
        double[] windowInputData = DSP.Math.Multiply(dSamples, windowCoefs);
        double windowScaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

        System.Numerics.Complex[] complexSpectrum = fft.Execute(windowInputData);
        //Convert to magnitude and multiply by the window scale factor to get our binned array
        double[] fftSpectrum = DSP.Math.Multiply(DSPLib.DSP.ConvertComplex.ToMagnitude(complexSpectrum), windowScaleFactor);
        float[] fFFTSpectrum = new float[fftSpectrum.Length];
        for (int i=0; i < fFFTSpectrum.Length; i++)
        {
            fFFTSpectrum[i] = (float)fftSpectrum[i];
        }
        return fFFTSpectrum;
    }
}
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843
