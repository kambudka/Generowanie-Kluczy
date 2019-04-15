using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Generowanie_Kluczy
{
    class Program
    {

        List<BitArray> KeyArray = new List<BitArray>();

        byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        public void ReadKey(string name)
        {
            byte[] PC1 = {57,49,41,33,25,17,9,1,58,50,42,34,26,18,10,2,59,51,43,35,27,19,11,3,60,52,44,36,
                          63,55,47,39,31,23,15,7,62,54,46,38,30,22,14,6,61,53,45,37,29,21,13,5,28,20,12,4};
            byte[] PC2 = {14,17,11,24,1,5,
                          3,28,15,6,21,10,
                          23,19,12,4,26,8,
                          16,7,27,20,13,2,
                          41,52,31,37,47,55,
                          30,40,51,45,33,48,
                          44,49,39,56,34,53,
                          46,42,50,36,29,32};
            byte[] SHIFT = {1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1};

            BinaryReader br;
            int number;
            byte readbyte;
            int bit = 0;
            string path = @"C:\Users\Zalgo\Desktop\Testy\Klucz";

            try { br = new BinaryReader(new FileStream(path, FileMode.Open)); }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return;
            }

            int poz = 0;
            BitArray FileKey = new BitArray(64);
            BitArray Key = new BitArray(56);
            BitArray KeyOut;
            BitArray C = new BitArray(28);
            BitArray D = new BitArray(28);

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                //Start PC1
                readbyte = br.ReadByte();
                BitArray mybits = new BitArray(new byte[] { readbyte });
                for (int j = 0; j < mybits.Length; j++)
                {
                    var mybit = mybits.Get(j);
                    //Console.WriteLine(mybit);
                    FileKey[poz] = mybit;
                    poz++;
                }

            }
            for(int j = 0; j < 56; j++)
            {
                Key[j] = FileKey.Get(PC1[j]-1);
                //Console.WriteLine(Key.Get(j));
            }
            for (int j = 0; j < 28; j++)
            {
                C[j] = Key.Get(j);
                D[j] = Key.Get(j+28);
                //Console.WriteLine(Key.Get(j));
            }
            bool tempC;
            bool tempD;
            //LShifty C i B
            for (int k = 0; k < 16; k++)
            {
                for (int j = 0; j < SHIFT[k]; j++)
                {
                    tempC = C[0];
                    for (int i = 1; i < C.Count; i++)
                    {
                        C[i - 1] = C[i];
                    }
                    C[C.Count - 1] = tempC;

                    tempD = D[0];
                    for (int i = 1; i < D.Count; i++)
                    {
                        D[i - 1] = D[i];
                    }
                    D[D.Count - 1] = tempD;
                }

                for (int j = 0; j < 28; j++)
                {
                    Key[j] = C.Get(j);
                    Key[j+28] = D.Get(j);
                    //Console.WriteLine(Key.Get(j));
                }

                KeyOut = new BitArray(48);
                for (int j = 0; j < 48; j++)
                {
                    KeyOut[j] = Key[PC2[j] - 1];
                    //Console.WriteLine(Key.Get(j));
                }

                KeyArray.Add(KeyOut);

            }



            br.Close();
        }

        public void GetBlock(string name)
        {
            byte[] IP = {58,50,42,34,26,18,10,2,
                         60,52,44,36,28,20,12,4,
                         62,54,46,38,30,22,14,6,
                         64,56,48,40,32,24,16,8,
                         57,49,41,33,25,17,9,1,
                         59,51,43,35,27,19,11,3,
                         61,53,45,37,29,21,13,5,
                         63,55,47,39,31,23,15,7};
            byte[] E = {32, 1, 2, 3, 4, 5,
                        4, 5, 6, 7, 8, 9,
                        8, 9, 10, 11, 12, 13,
                        12, 13, 14, 15, 16, 17,
                        16, 17, 18, 19, 20, 21,
                        20, 21, 22, 23, 24, 25,
                        24, 25, 26, 27, 28, 29,
                        28, 29, 30, 31, 32, 1};

            #region S-tables
            byte[] S1 = {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
                        0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
                        4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
                        15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13};

            byte[] S2 = {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
                        3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
                        0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
                        13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9};

            byte[] S3 = {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
                        13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
                        13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
                        1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12};

            byte[] S4 = {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
                        13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
                        10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
                        3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14};

            byte[] S5 = {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
                        14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
                        4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
                        11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3};

            byte[] S6 = {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
                        10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
                        9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
                        4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13};

            byte[] S7 = {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
                        13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
                        1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
                        6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12};

            byte[] S8 = {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
                        1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
                        7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
                        2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11};
            #endregion

            byte[] SHIFT = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

            BinaryReader br;
            int number;
            byte readbyte;
            int bit = 0;
            string path = @"C:\Users\Zalgo\Desktop\Testy\test3.bin";

            try { br = new BinaryReader(new FileStream(path, FileMode.Open)); }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return;
            }

            int poz = 0;
            BitArray FileBlock = new BitArray(64);
            BitArray Block = new BitArray(64);
            BitArray KeyOut;
            BitArray L = new BitArray(32);
            BitArray Ltemp = new BitArray(32);
            BitArray R = new BitArray(32);
            BitArray R48 = new BitArray(48);
            BitArray XORED = new BitArray(48);
            BitArray BIT6 = new BitArray(6);

            //while (br.BaseStream.Position != br.BaseStream.Length)
            //{
                poz = 0;
                //Start PC1
                for (int i = 0; i < 8; i++)
                {
                    readbyte = br.ReadByte();
                    BitArray mybits = new BitArray(new byte[] { readbyte });
                    for (int j = 0; j < mybits.Length; j++)
                    {
                        var mybit = mybits.Get(j);
                        //Console.WriteLine(mybit);
                        FileBlock[poz] = mybit;
                        poz++;
                    }
                }

                //Initial Permutation
                for (int j = 0; j < 64; j++)
                {
                    Block[j] = FileBlock.Get(IP[j]-1);
                    //Console.WriteLine(Key.Get(j));
                }
                //Spliting L and R
                for (int j = 0; j < 32; j++)
                {
                    L[j] = Block.Get(j);
                    R[j] = Block.Get(j + 32);
                    //Console.WriteLine(Key.Get(j));
                }



                //32 R Block -> 48 R Block
                for (int j = 0; j < 48; j++)
                {
                    R48[j] = R.Get(E[j]-1);
                    //Console.WriteLine(Key.Get(j));
                }
                //R Block XOR KEY
                XORED = R48.Xor(KeyArray[0]);

                bool[] index = new bool[8] { false, false, false, false, false, false, false, false };
                int offset;
                int offsetr;
                byte wiersz;
                byte kolumna;
                BitArray nowy = new BitArray(4);
                // Do lewego przypisujemy prawy
                Ltemp = R;
                //Split into 8 - 6bit
                for (int i = 0; i < 8; i++)
                {
                    offset = 6 * i;
                    BitArray indexo = new BitArray(index);
                    indexo[0] = XORED[0 + offset];
                    indexo[1] = XORED[5 + offset];

                    wiersz = ConvertToByte(indexo);
                    //wiersz += 16;

                    indexo[3] = XORED[1 + offset];
                    indexo[2] = XORED[2 + offset];
                    indexo[1] = XORED[3 + offset];
                    indexo[0] = XORED[4 + offset];
                    kolumna = ConvertToByte(indexo);
                    
                    switch(i)
                    {
                    case 0:
                        nowy = new BitArray(new byte[] { S1[kolumna + 16 * wiersz] });
                        break;
                    case 1:
                        nowy = new BitArray(new byte[] { S2[kolumna + 16 * wiersz] });
                        break;
                    case 2:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    case 3:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    case 4:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    case 5:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    case 6:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    case 7:
                        nowy = new BitArray(new byte[] { S3[kolumna + 16 * wiersz] });
                        break;
                    }

                    offsetr = 4 * i;

                        R[offsetr] = nowy[3];
                        Console.Write(R.Get(offsetr));
                        R[offsetr + 1] = nowy[2];
                        Console.Write(R.Get(offsetr + 1));
                        R[offsetr + 2] = nowy[1];
                        Console.Write(R.Get(offsetr + 2));
                        R[offsetr + 3] = nowy[0];
                        Console.WriteLine(R.Get(offsetr + 3));

                    
                }

                R = R.Xor(L);
                L = Ltemp;

            //}
            

            br.Close();

        }




        static void Main(string[] args)
        {
            Program program = new Program();
            string code;
            string seed;
            string name;

            Console.WriteLine("Podaj nazwe pliku klucza");
            name = Console.ReadLine();
            program.ReadKey(name);
            Console.WriteLine("Podaj nazwe pliku wejscia");
            name = Console.ReadLine();
            program.GetBlock(name);
            Console.ReadKey();
        }
    }
}
