// 
//  Copyright (C) 2011 Robin Duerden (rduerden@gmail.com)
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// 
// 
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
namespace ConsoleTests
{
    // adapted from http://www.codeproject.com/KB/graphics/exifextractor.aspx
    public class ExifExtractor
    {
        static ExifExtractor()
        {
            initTable();
        }

        private readonly Dictionary<string,string> properties;

        public Dictionary<string,string> Properties { get { return properties; } }

        public ExifExtractor( Stream stream )
        {
            properties = new Dictionary<string, string>();

            Bitmap bm = new Bitmap( stream );
            using( bm ) build( bm );
        }

        private void build( Bitmap bitmap )
        {
            properties.Clear();
            //
            Encoding ascii = Encoding.ASCII;
            //
            foreach( System.Drawing.Imaging.PropertyItem p in bitmap.PropertyItems )
            {
                string name;
                if( ! exifTable.TryGetValue( p.Id, out name )) continue;

                string v;

                //
                //1 = BYTE An 8-bit unsigned integer.,
                if( p.Type == 0x1 )
                {
                    v = p.Value[0].ToString();
                }
                    //2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL.,
                else if( p.Type == 0x2 )
                {
                    // string
                    v = ascii.GetString(p.Value);
                }
                    //3 = SHORT A 16-bit (2 -byte) unsigned integer,
                else if( p.Type == 0x3 )
                {

                    // orientation // lookup table
                    switch( p.Id )
                    {
                        case 0x8827: // ISO
                            v = "ISO-" + BitConverter.ToUInt16(p.Value, 0).ToString();
                            break;
                        case 0xA217: // sensing method
                        {
                            switch( BitConverter.ToUInt16(p.Value, 0) )
                            {
                                case 1: v = "Not defined"; break;
                                case 2: v = "One-chip color area sensor"; break;
                                case 3: v = "Two-chip color area sensor"; break;
                                case 4: v = "Three-chip color area sensor"; break;
                                case 5: v = "Color sequential area sensor"; break;
                                case 7: v = "Trilinear sensor"; break;
                                case 8: v = "Color sequential linear sensor"; break;
                                default: v =" reserved"; break;
                            }
                        }
                            break;
                        case 0x8822: // aperture
                        switch( BitConverter.ToUInt16(p.Value, 0 ))
                        {
                            case 0: v = "Not defined"; break;
                            case 1: v = "Manual"; break;
                            case 2: v = "Normal program"; break;
                            case 3: v = "Aperture priority"; break;
                            case 4: v = "Shutter priority"; break;
                            case 5: v = "Creative program (biased toward depth of field)"; break;
                            case 6: v = "Action program (biased toward fast shutter speed)"; break;
                            case 7: v = "Portrait mode (for closeup photos with the background out of focus)"; break;
                            case 8: v = "Landscape mode (for landscape photos with the background in focus)"; break;
                            default: v = "reserved"; break;
                        }
                            break;
                        case 0x9207: // metering mode
                        switch( BitConverter.ToUInt16(p.Value, 0 ))
                        {
                            case 0: v = "unknown";break;
                            case 1: v = "Average";break;
                            case 2: v = "CenterWeightedAverage"; break;
                            case 3: v = "Spot"; break;
                            case 4: v = "MultiSpot"; break;
                            case 5: v = "Pattern"; break;
                            case 6: v = "Partial"; break;
                            case 255: v = "Other"; break;
                            default: v = "reserved"; break;
                        }
                            break;
                        case 0x9208: // light source
                        {
                            switch(BitConverter.ToUInt16(p.Value, 0 ))
                            {
                                case 0:v = "unknown";break;
                                case 1:v = "Daylight";break;
                                case 2:v = "Fluorescent";break;
                                case 3:v = "Tungsten";break;
                                case 17:v = "Standard light A";break;
                                case 18:v = "Standard light B";break;
                                case 19:v = "Standard light C";break;
                                case 20:v = "D55";break;
                                case 21:v = "D65";break;
                                case 22:v = "D75";break;
                                case 255:v = "other";break;
                                default:v = "reserved";break;
                            }
                        }
                            break;
                        case 0x9209:
                        {
                            switch(BitConverter.ToUInt16(p.Value, 0 ))
                            {
                                case 0: v = "Flash did not fire"; break;
                                case 1: v = "Flash fired"; break;
                                case 5: v = "Strobe return light not detected"; break;
                                case 7: v = "Strobe return light detected"; break;
                                default: v = "reserved"; break;
                            }
                        }
                            break;
                        default:
                            v = BitConverter.ToUInt16(p.Value, 0 ).ToString();
                            break;
                    }
                }
                    //4 = LONG A 32-bit (4 -byte) unsigned integer,
                else if( p.Type == 0x4 )
                {
                    // orientation // lookup table
                    v = BitConverter.ToUInt32(p.Value, 0).ToString();
                }
                    //5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator.,
                else if( p.Type == 0x5 )
                {
                    // rational
                    byte []n = new byte[p.Len/2];
                    byte []d = new byte[p.Len/2];
                    Array.Copy(p.Value,0,n,0,p.Len/2);
                    Array.Copy(p.Value,p.Len/2,d,0,p.Len/2);
                    uint a = BitConverter.ToUInt32(n, 0);
                    uint b = BitConverter.ToUInt32(d, 0);
                    Rational r = new Rational(a,b);
                    //
                    //convert here
                    //
                    switch( p.Id )
                    {
                        case 0x9202: // aperture
                            v = "F/" + Math.Round(Math.Pow(Math.Sqrt(2),r.ToDouble()),2).ToString();
                            break;
                        case 0x920A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829D: // F-number
                            v = "F/" + r.ToDouble().ToString();
                            break;
                        default:
                            v= r.ToString("/");
                            break;
                    }
                    
                }
                    //7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition,
                else if( p.Type == 0x7 )
                {                   
                    switch (p.Id )
                    { 
                        case 0xA300:
                        {
                            if( p.Value[0] == 3 )
                            { 
                                v = "DSC"; 
                            }
                            else
                            {
                                v = "reserved"; 
                            }
                            break;
                        }
                        case 0xA301:
                            if( p.Value[0] == 1 )
                                v = "A directly photographed image";
                            else
                                v = "Not a directly photographed image";
                            break;
                        default:
                            v = "-";
                            break;
                    } 
                }               
                    //9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation),
                else if( p.Type == 0x9 )
                {
                    v = BitConverter.ToInt32(p.Value, 0).ToString();
                }
                    //10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the
                    //denominator.
                else if( p.Type == 0xA )
                {

                    // rational
                    byte []n = new byte[p.Len/2];
                    byte []d = new byte[p.Len/2];
                    Array.Copy(p.Value,0,n,0,p.Len/2);
                    Array.Copy(p.Value,p.Len/2,d,0,p.Len/2);
                    int a = BitConverter.ToInt32(n,0);
                    int b = BitConverter.ToInt32(d,0);
                    Rational r = new Rational(a,b);
                    //
                    // convert here
                    //
                    switch( p.Id )
                    {
                        case 0x9201: // shutter speed
                            v = "1/" +Math.Round( Math.Pow(2,r.ToDouble()),2).ToString();
                            break;
                        case 0x9203:
                            v = Math.Round(r.ToDouble(),4).ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }
                }
                else v = null;

                if( v != null && ! properties.ContainsKey( name ))
                    properties.Add(name,v);
            }
        }

        private static readonly Dictionary<int, string> exifTable = new Dictionary<int, string>();

        private static void initTable()
        {
            exifTable.Add(0x8769,"Exif IFD");
            exifTable.Add(0x8825,"Gps IFD");
            exifTable.Add(0xFE,"New Subfile Type");
            exifTable.Add(0xFF,"Subfile Type");
            exifTable.Add(0x100,"Image Width");
            exifTable.Add(0x101,"Image Height");
            exifTable.Add(0x102,"Bits Per Sample");
            exifTable.Add(0x103,"Compression");
            exifTable.Add(0x106,"Photometric Interp");
            exifTable.Add(0x107,"Thresh Holding");
            exifTable.Add(0x108,"Cell Width");
            exifTable.Add(0x109,"Cell Height");
            exifTable.Add(0x10A,"Fill Order");
            exifTable.Add(0x10D,"Document Name");
            exifTable.Add(0x10E,"Image Description");
            exifTable.Add(0x10F,"Equip Make");
            exifTable.Add(0x110,"Equip Model");
            exifTable.Add(0x111,"Strip Offsets");
            exifTable.Add(0x112,"Orientation");
            exifTable.Add(0x115,"Samples PerPixel");
            exifTable.Add(0x116,"Rows Per Strip");
            exifTable.Add(0x117,"Strip Bytes Count");
            exifTable.Add(0x118,"Min Sample Value");
            exifTable.Add(0x119,"Max Sample Value");
            exifTable.Add(0x11A,"X Resolution");
            exifTable.Add(0x11B,"Y Resolution");
            exifTable.Add(0x11C,"Planar Config");
            exifTable.Add(0x11D,"Page Name");
            exifTable.Add(0x11E,"X Position");
            exifTable.Add(0x11F,"Y Position");
            exifTable.Add(0x120,"Free Offset");
            exifTable.Add(0x121,"Free Byte Counts");
            exifTable.Add(0x122,"Gray Response Unit");
            exifTable.Add(0x123,"Gray Response Curve");
            exifTable.Add(0x124,"T4 Option");
            exifTable.Add(0x125,"T6 Option");
            exifTable.Add(0x128,"Resolution Unit");
            exifTable.Add(0x129,"Page Number");
            exifTable.Add(0x12D,"Transfer Funcition");
            exifTable.Add(0x131,"Software Used");
            exifTable.Add(0x132,"Date Time");
            exifTable.Add(0x13B,"Artist");
            exifTable.Add(0x13C,"Host Computer");
            exifTable.Add(0x13D,"Predictor");
            exifTable.Add(0x13E,"White Point");
            exifTable.Add(0x13F,"Primary Chromaticities");
            exifTable.Add(0x140,"ColorMap");
            exifTable.Add(0x141,"Halftone Hints");
            exifTable.Add(0x142,"Tile Width");
            exifTable.Add(0x143,"Tile Length");
            exifTable.Add(0x144,"Tile Offset");
            exifTable.Add(0x145,"Tile ByteCounts");
            exifTable.Add(0x14C,"InkSet");
            exifTable.Add(0x14D,"Ink Names");
            exifTable.Add(0x14E,"Number Of Inks");
            exifTable.Add(0x150,"Dot Range");
            exifTable.Add(0x151,"Target Printer");
            exifTable.Add(0x152,"Extra Samples");
            exifTable.Add(0x153,"Sample Format");
            exifTable.Add(0x154,"S Min Sample Value");
            exifTable.Add(0x155,"S Max Sample Value");
            exifTable.Add(0x156,"Transfer Range");
            exifTable.Add(0x200,"JPEG Proc");
            exifTable.Add(0x201,"JPEG InterFormat");
            exifTable.Add(0x202,"JPEG InterLength");
            exifTable.Add(0x203,"JPEG RestartInterval");
            exifTable.Add(0x205,"JPEG LosslessPredictors");
            exifTable.Add(0x206,"JPEG PointTransforms");
            exifTable.Add(0x207,"JPEG QTables");
            exifTable.Add(0x208,"JPEG DCTables");
            exifTable.Add(0x209,"JPEG ACTables");
            exifTable.Add(0x211,"YCbCr Coefficients");
            exifTable.Add(0x212,"YCbCr Subsampling");
            exifTable.Add(0x213,"YCbCr Positioning");
            exifTable.Add(0x214,"REF Black White");
            exifTable.Add(0x8773,"ICC Profile");
            exifTable.Add(0x301,"Gamma");
            exifTable.Add(0x302,"ICC Profile Descriptor");
            exifTable.Add(0x303,"SRGB RenderingIntent");
            exifTable.Add(0x320,"Image Title");
            exifTable.Add(0x8298,"Copyright");
            exifTable.Add(0x5001,"Resolution X Unit");
            exifTable.Add(0x5002,"Resolution Y Unit");
            exifTable.Add(0x5003,"Resolution X LengthUnit");
            exifTable.Add(0x5004,"Resolution Y LengthUnit");
            exifTable.Add(0x5005,"Print Flags");
            exifTable.Add(0x5006,"Print Flags Version");
            exifTable.Add(0x5007,"Print Flags Crop");
            exifTable.Add(0x5008,"Print Flags Bleed Width");
            exifTable.Add(0x5009,"Print Flags Bleed Width Scale");
            exifTable.Add(0x500A,"Halftone LPI");
            exifTable.Add(0x500B,"Halftone LPIUnit");
            exifTable.Add(0x500C,"Halftone Degree");
            exifTable.Add(0x500D,"Halftone Shape");
            exifTable.Add(0x500E,"Halftone Misc");
            exifTable.Add(0x500F,"Halftone Screen");
            exifTable.Add(0x5010,"JPEG Quality");
            exifTable.Add(0x5011,"Grid Size");
            exifTable.Add(0x5012,"Thumbnail Format");
            exifTable.Add(0x5013,"Thumbnail Width");
            exifTable.Add(0x5014,"Thumbnail Height");
            exifTable.Add(0x5015,"Thumbnail ColorDepth");
            exifTable.Add(0x5016,"Thumbnail Planes");
            exifTable.Add(0x5017,"Thumbnail RawBytes");
            exifTable.Add(0x5018,"Thumbnail Size");
            exifTable.Add(0x5019,"Thumbnail CompressedSize");
            exifTable.Add(0x501A,"Color Transfer Function");
            exifTable.Add(0x501B,"Thumbnail Data");
            exifTable.Add(0x5020,"Thumbnail ImageWidth");
            exifTable.Add(0x502,"Thumbnail ImageHeight");
            exifTable.Add(0x5022,"Thumbnail BitsPerSample");
            exifTable.Add(0x5023,"Thumbnail Compression");
            exifTable.Add(0x5024,"Thumbnail PhotometricInterp");
            exifTable.Add(0x5025,"Thumbnail ImageDescription");
            exifTable.Add(0x5026,"Thumbnail EquipMake");
            exifTable.Add(0x5027,"Thumbnail EquipModel");
            exifTable.Add(0x5028,"Thumbnail StripOffsets");
            exifTable.Add(0x5029,"Thumbnail Orientation");
            exifTable.Add(0x502A,"Thumbnail SamplesPerPixel");
            exifTable.Add(0x502B,"Thumbnail RowsPerStrip");
            exifTable.Add(0x502C,"Thumbnail StripBytesCount");
            exifTable.Add(0x502D,"Thumbnail ResolutionX");
            exifTable.Add(0x502E,"Thumbnail ResolutionY");
            exifTable.Add(0x502F,"Thumbnail PlanarConfig");
            exifTable.Add(0x5030,"Thumbnail ResolutionUnit");
            exifTable.Add(0x5031,"Thumbnail TransferFunction");
            exifTable.Add(0x5032,"Thumbnail SoftwareUsed");
            exifTable.Add(0x5033,"Thumbnail DateTime");
            exifTable.Add(0x5034,"Thumbnail Artist");
            exifTable.Add(0x5035,"Thumbnail WhitePoint");
            exifTable.Add(0x5036,"Thumbnail PrimaryChromaticities");
            exifTable.Add(0x5037,"Thumbnail YCbCrCoefficients");
            exifTable.Add(0x5038,"Thumbnail YCbCrSubsampling");
            exifTable.Add(0x5039,"Thumbnail YCbCrPositioning");
            exifTable.Add(0x503A,"Thumbnail RefBlackWhite");
            exifTable.Add(0x503B,"Thumbnail CopyRight");
            exifTable.Add(0x5090,"Luminance Table");
            exifTable.Add(0x5091,"Chrominance Table");
            exifTable.Add(0x5100,"Frame Delay");
            exifTable.Add(0x5101,"Loop Count");
            exifTable.Add(0x5110,"Pixel Unit");
            exifTable.Add(0x5111,"Pixel PerUnit X");
            exifTable.Add(0x5112,"Pixel PerUnit Y");
            exifTable.Add(0x5113,"Palette Histogram");
            exifTable.Add(0x829A,"Exposure Time");
            exifTable.Add(0x829D,"F-Number");
            exifTable.Add(0x8822,"Exposure Prog");
            exifTable.Add(0x8824,"Spectral Sense");
            exifTable.Add(0x8827,"ISO Speed");
            exifTable.Add(0x8828,"OECF");
            exifTable.Add(0x9000,"Ver");
            exifTable.Add(0x9003,"DTOrig");
            exifTable.Add(0x9004,"DTDigitized");
            exifTable.Add(0x9101,"CompConfig");
            exifTable.Add(0x9102,"CompBPP");
            exifTable.Add(0x9201,"Shutter Speed");
            exifTable.Add(0x9202,"Aperture");
            exifTable.Add(0x9203,"Brightness");
            exifTable.Add(0x9204,"Exposure Bias");
            exifTable.Add(0x9205,"MaxAperture");
            exifTable.Add(0x9206,"SubjectDist");
            exifTable.Add(0x9207,"Metering Mode");
            exifTable.Add(0x9208,"LightSource");
            exifTable.Add(0x9209,"Flash");
            exifTable.Add(0x920A,"FocalLength");
            exifTable.Add(0x927C,"Maker Note");
            exifTable.Add(0x9286,"User Comment");
            exifTable.Add(0x9290,"DTSubsec");
            exifTable.Add(0x9291,"DTOrigSS");
            exifTable.Add(0x9292,"DTDigSS");
            exifTable.Add(0xA000,"FPXVer");
            exifTable.Add(0xA001,"ColorSpace");
            exifTable.Add(0xA002,"PixXDim");
            exifTable.Add(0xA003,"PixYDim");
            exifTable.Add(0xA004,"RelatedWav");
            exifTable.Add(0xA005,"Interop");
            exifTable.Add(0xA20B,"FlashEnergy");
            exifTable.Add(0xA20C,"SpatialFR");
            exifTable.Add(0xA20E,"FocalXRes");
            exifTable.Add(0xA20F,"FocalYRes");
            exifTable.Add(0xA210,"FocalResUnit");
            exifTable.Add(0xA214,"Subject Loc");
            exifTable.Add(0xA215,"Exposure Index");
            exifTable.Add(0xA217,"Sensing Method");
            exifTable.Add(0xA300,"FileSource");
            exifTable.Add(0xA301,"SceneType");
            exifTable.Add(0xA302,"CfaPattern");
            exifTable.Add(0x0,"Gps Ver");
            exifTable.Add(0x1,"Gps LatitudeRef");
            exifTable.Add(0x2,"Gps Latitude");
            exifTable.Add(0x3,"Gps LongitudeRef");
            exifTable.Add(0x4,"Gps Longitude");
            exifTable.Add(0x5,"Gps AltitudeRef");
            exifTable.Add(0x6,"Gps Altitude");
            exifTable.Add(0x7,"Gps GpsTime");
            exifTable.Add(0x8,"Gps GpsSatellites");
            exifTable.Add(0x9,"Gps GpsStatus");
            exifTable.Add(0xA,"Gps GpsMeasureMode");
            exifTable.Add(0xB,"Gps GpsDop");
            exifTable.Add(0xC,"Gps SpeedRef");
            exifTable.Add(0xD,"Gps Speed");
            exifTable.Add(0xE,"Gps TrackRef");
            exifTable.Add(0xF,"Gps Track");
            exifTable.Add(0x10,"Gps ImgDirRef");
            exifTable.Add(0x11,"Gps ImgDir");
            exifTable.Add(0x12,"Gps MapDatum");
            exifTable.Add(0x13,"Gps DestLatRef");
            exifTable.Add(0x14,"Gps DestLat");
            exifTable.Add(0x15,"Gps DestLongRef");
            exifTable.Add(0x16,"Gps DestLong");
            exifTable.Add(0x17,"Gps DestBearRef");
            exifTable.Add(0x18,"Gps DestBear");
            exifTable.Add(0x19,"Gps DestDistRef");
            exifTable.Add(0x1A,"Gps DestDist");

        }
        internal class Rational
        {
            private int n;
            private int d;
            public Rational(int n, int d)
            {
                this.n = n;
                this.d = d;
                simplify(ref this.n, ref this.d);
            }
            public Rational(uint n, uint d)
            {
                this.n = Convert.ToInt32(n);
                this.d = Convert.ToInt32(d);

                simplify(ref this.n, ref this.d);
            }
            public Rational()
            {
                this.n= this.d=0;
            }
            public string ToString(string sp)
            {
                if( sp == null ) sp = "/";
                return n.ToString() + sp + d.ToString();
            }
            public double ToDouble()
            {
                if( d == 0 )
                    return 0.0;

                return Math.Round(Convert.ToDouble(n)/Convert.ToDouble(d),2);
            }
            private void simplify( ref int a, ref int b )
            {
                if( a== 0 || b == 0 )
                    return;

                int gcd = euclid(a,b);
                a /= gcd;
                b /= gcd;
            }
            private int euclid(int a, int b)
            {
                if(b==0)    
                    return a;
                else        
                    return euclid(b,a%b);
            }
        }

    }
}

