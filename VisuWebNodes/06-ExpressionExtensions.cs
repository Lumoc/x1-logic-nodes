﻿using System;

namespace Recomedia_de.Logic.VisuWeb
{
  // Angle conversion functions degrees <--> radian
  public static class Angle
  {
    public static double Deg(double rad)
    {
      return rad * (180.0 / Math.PI);
    }
    public static double Rad(double deg)
    {
      return deg / (180.0 / Math.PI);
    }
  }

  // Lighting conversions
  public static class Light
  {
    private const double D360 = 256.0;      // 360°
    private const double  D60 = D360 / 6.0; //  60°

    private static byte ToHue(byte r, byte g, byte b, int max, double span)
    {
      double hf = 0.0;

      if (span != 0.0)
      {
        if (max == r)
        {
          hf = D60 * (0.0 + ((double)g - (double)b) / span);
        }
        else if (max == g)
        {
          hf = D60 * (2.0 + ((double)b - (double)r) / span);
        }
        else // if (max == b)
        {
          hf = D60 * (4.0 + ((double)r - (double)g) / span);
        }
        if (hf < 0.0)
        {
          hf += D360;
        }
      }
      return (byte)Math.Round(hf, MidpointRounding.AwayFromZero);
    }

    public static int HSV(byte h, byte s, byte v)
    {
      return ((int)h << 16) + ((int)s << 8) + (int)v;
    }

    public static byte HSVToH(int hsv)
    {
      return (byte)((hsv >> 16) & 0xFF);
    }

    public static byte HSVToS(int hsv)
    {
      return (byte)((hsv >> 8) & 0xFF);
    }

    public static byte HSVToV(int hsv)
    {
      return (byte)(hsv & 0xFF);
    }

    public static int RGBToHSV(int rgb)
    {
      return RGBToHSV(RGBToR(rgb), RGBToG(rgb), RGBToB(rgb));
    }

    public static int RGBToHSV(byte r, byte g, byte b)
    {
      byte max = Math.Max(Math.Max(r, g), b);
      byte min = Math.Min(Math.Min(r, g), b);
      double span = (double)max - (double)min;
      byte h = ToHue(r, g, b, max, span);
      byte s = 0;
      if (max != 0)
      {
        double sf = 255.0 * span / max;
        s = (byte)Math.Round(sf, MidpointRounding.AwayFromZero);
      }
      return HSV(h, s, max);
    }

    private static int ToRGB(int hi, byte v, byte p, byte q, byte t)
    {
      switch (hi)
      {
        case 0:
          return RGB(v, t, p);
        case 1:
          return RGB(q, v, p);
        case 2:
          return RGB(p, v, t);
        case 3:
          return RGB(p, q, v);
        case 4:
          return RGB(t, p, v);
        case 5:
          return RGB(v, p, q);
        default:
          return RGB(255, 255, 255);  // should never happen
      }
    }

    public static int HSVToRGB(int hsv)
    {
      return HSVToRGB(HSVToH(hsv), HSVToS(hsv), HSVToV(hsv));
    }

    public static int HSVToRGB(byte h, byte s, byte v)
    {
      if (s == 0)
      {
        return RGB(v, v, v);
      }
      else
      {
        double h2 = (double)h / D60;  // h/60°
        int hi = (int)Math.Floor(h2);
        double f = h2 - hi;
        double pf = (double)v * (255 - s) / 255.0;
        double qf = (double)v * (255 - s * f) / 255.0;
        double tf = (double)v * (255 - s * (1.0 - f)) / 255.0;
        byte p = (byte)Math.Round(pf, MidpointRounding.AwayFromZero);
        byte q = (byte)Math.Round(qf, MidpointRounding.AwayFromZero);
        byte t = (byte)Math.Round(tf, MidpointRounding.AwayFromZero);
        return ToRGB(hi, v, p, q, t);
      }
    }

    public static int RGB(byte r, byte g, byte b)
    {
      return ((int)r << 16) + ((int)g << 8) + (int)b;
    }

    public static byte RGBToR(int rgb)
    {
      return (byte)((rgb >> 16) & 0xFF);
    }

    public static byte RGBToG(int rgb)
    {
      return (byte)((rgb >> 8) & 0xFF);
    }

    public static byte RGBToB(int rgb)
    {
      return (byte)(rgb & 0xFF);
    }

    public static long RGBW(int rgb)
    {
      byte r = RGBToR(rgb);
      byte g = RGBToG(rgb);
      byte b = RGBToB(rgb);
      return RGBW(r, g, b);
    }

    public static long RGBW(byte r, byte g, byte b)
    {
      byte min = Math.Min(Math.Min(r, g), b);
      return RGBW(r - min, g - min, b - min, min);
    }

    public static long RGBW(int r, int g, int b, int w)
    {
      return( (((r < 0) || (r > 0xff)) ? 0 : (((long)1 << 35) + ((long)r << 24))) +
              (((g < 0) || (g > 0xff)) ? 0 : (((long)1 << 34) + ((long)g << 16))) +
              (((b < 0) || (b > 0xff)) ? 0 : (((long)1 << 33) + ((long)b << 8))) +
              (((w < 0) || (w > 0xff)) ? 0 : (((long)1 << 32) + ((long)w))) );
    }

    public static short RGBWToR(long rgbw)
    {
      return (short)((((rgbw >> 35) & 1) == 0) ? -1 : ((rgbw >> 24) & 0xFF));
    }

    public static short RGBWToG(long rgbw)
    {
      return (short)((((rgbw >> 34) & 1) == 0) ? -1 : ((rgbw >> 16) & 0xFF));
    }

    public static short RGBWToB(long rgbw)
    {
      return (short)((((rgbw >> 33) & 1) == 0) ? -1 : ((rgbw >> 8) & 0xFF));
    }

    public static short RGBWToW(long rgbw)
    {
      return (short)((((rgbw >> 32) & 1) == 0) ? -1 : (rgbw & 0xFF));
    }
  }

  /* 
   * Anwendungsfall: Berechnung des Taupunktes aus Temperatur und Luftfeuchtigkeit
   * 
   * Eingabegrößen:
   *   rF = relative Luftfeuchte
   *   T  = Temperatur (in °C wenn nicht anders angegeben)
   * 
   * Parameter:
   *   a = 7.5, b = 237.3 für T >= 0
   *   a = 7.6, b = 240.7 für T < 0 über Wasser (Taupunkt)
   *   a = 9.5, b = 265.5 für T < 0 über Eis (Frostpunkt)
   *   R  = 8314.3 J/(kmol*K) (universelle Gaskonstante)
   *   mw = 18.016 kg/kmol    (Molekulargewicht des Wasserdampfes)
   * 
   * Näherungsformeln basierend auf der Magnusformel:
   *   absoluteFeuchte[g/m^3](rF,T) = 10^5 * mw/R * DD(rF,T) / TK(T)
   *   Taupunkttemperatur[°C](rF,T) = b*v(rF,T)/(a-v(rF,T)) = 1 / ( (a/(b*v(rF,T))) - (1/b) )
   * wobei:
   *   Sättigungsdampfdruck[hPa] SDD(T)   = 6.1078 * 10^((a*T)/(b+T))
   *   Dampfdruck[hPa]           DD(rF,T) = rF/100 * SDD(T)
   *   absolute Temperatur       TK(T)    = T + 273.15
   *   v(rF,T) = log10(DD(rF,T)/6.1078)
   * 
   * Quelle: http://www.wetterochs.de/wetter/feuchte.html
   */
  public static class Hlk
  {
    private static double getTk(double temp)
    {
      return temp + 273.15;
    }
    private static double getA(double temp)
    {
      return ((temp >= 0.0) ? 7.5 : 7.6);
    }
    private static double getB(double temp)
    {
      return ((temp >= 0.0) ? 237.3 : 240.7);
    }
    private static double getSdd(double temp)
    {
      var a = getA(temp);
      var b = getB(temp);
      return 6.1078 * Math.Pow(10, ((a * temp) / (b + temp)));
    }
    private static double getDd(double temp, double relHum)
    {
      return relHum / 100 * getSdd(temp);
    }
    private static double getV(double temp , double relHum)
    {
      return Math.Log10(getDd(temp, relHum) / 6.1078);
    }

    public static double AbsHumidity(double temp, double relHum)
    {
      const double mw = 18.016;
      const double r = 8314.3;
      var dd = getDd(temp, relHum);
      var tk = getTk(temp);
      return 100000 * mw / r * dd / tk;
    }
    public static double DewPoint(double temp, double relHum)
    {
      var a = getA(temp);
      var b = getB(temp);
      var v = getV(temp, relHum);
      return (b * v) / (a - v);
    }
  }

}
