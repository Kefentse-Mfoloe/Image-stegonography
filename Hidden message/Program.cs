using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hidden_message
{
    class Program
    {
        public static string StringToBinary(string txt)
        {
            string res = "";
            foreach (char c in txt)
            {
                res = res + Convert.ToString(c, 2).PadLeft(8, '0');
                Console.WriteLine("{0} = {1}", Convert.ToString(c, 2).PadLeft(8, '0'), c);
            }  
            return res;
        }

        public static bool isTerminatorFound(string str)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < str.Length; i += 8)
            {
                int index = byteList.Count - 1;
                if (index > -1 && byteList[index] == '#')
                    return true;
                byteList.Add(Convert.ToByte(str.Substring(i, 8), 2));
            }
            return false;
        }
        
        //This function translates a binary message into a string until a '#' appears
        public static string BinaryToString(string str)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < str.Length; i += 8)
            {
                int index = byteList.Count - 1;
                if (index > -1 && byteList[index] == '#')
                    break;
                byteList.Add(Convert.ToByte(str.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        public static void imgArrToPPM(string[] imgArr)
        {
            Directory.CreateDirectory(@"C:\Users\g13m3615\Documents\Image processing\encrypted image");
            string path = @"C:\Users\g13m3615\Documents\Image processing\encrypted image\encryptedImg.ppm";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (string x in imgArr)
                    {
                        sw.WriteLine(x);
                    }
                }
            }
        }

        public static string[] EncryptImage(string msg, string[] img)
        {
            string[] new_img = new string[img.Length];
            new_img[0] = img[0];
            new_img[1] = img[1];
            new_img[2] = img[2];
            new_img[3] = img[3];
            
            // Binary version of the string you typed in
            string bin = StringToBinary(msg);


            int bin_index = 0;
            for(int img_index = 4; img_index < img.Length; img_index++)
            {
                int rgbVal = Convert.ToInt32(img[img_index]);

                if (bin_index < bin.Length)
                {
                    // We want to associate even rgb pixel values with the 1s of the binary text
                    if (bin[bin_index] == '1')
                    {
                        // The rgbVal should be an odd number for us to know that it is a 1
                        if (rgbVal % 2 != 1)
                        {
                            rgbVal--;
                            new_img[img_index] = Convert.ToString(rgbVal);
                        }
                        else new_img[img_index] = Convert.ToString(rgbVal);
                    }
                    // We want to associate odd rgb pixel values with the 0s of the binary text
                    else
                    {
                        // The rgbVal should be an even number for us to know that it is a 0
                        if (rgbVal % 2 != 0)
                        {
                            rgbVal--;
                            new_img[img_index] = Convert.ToString(rgbVal);
                        }
                        else new_img[img_index] = Convert.ToString(rgbVal);
                    }
                    bin_index++;
                }
                else
                {
                    new_img[img_index] = img[img_index];
                }
            }
            imgArrToPPM(new_img);
            return new_img;
        }

        public static string DecryptImage(string[] img, int messageLength)
        {
            string bin = "";
            string msg = "";
            int x = img.Length;
            for(int index = 4; index < messageLength + 4; index++)
            {
                if (Convert.ToInt32(img[index]) % 2 == 1)
                    bin = bin + "1";
                else
                    bin = bin + "0";
            }
            msg = BinaryToString(bin);
            return msg;
        }

        static void Main(string[] args)
        {
            // Let's read the message from a file
            Console.WriteLine("Enter your text file: ");
            string randomTxtFile = Console.ReadLine();
            string randomtext = System.IO.File.ReadAllText(@"C:\Users\g13m3615\Documents\Image processing\" + randomTxtFile);

            // Let's read the image 
            string[] rgblines = System.IO.File.ReadAllLines(@"C:\Users\g13m3615\Pictures\seal.ppm");

            // Lets encrypt the image 
            Console.WriteLine("Message is being encrypted");
            string[] new_img = EncryptImage(randomtext, rgblines);
            Console.WriteLine("Message encrypted. Check encrypted image. \nPress Enter to begin decryption.");
            Console.ReadLine();

            // Let's decrypt the image
            string msg = DecryptImage(new_img, randomtext.Length * 8);
            Console.WriteLine("Message decrypted. Check encrypted image. \nPress Enter to view message.");
            Console.ReadLine();
            Console.WriteLine("{0}", msg);
            Console.ReadLine();
        }
    }
}
