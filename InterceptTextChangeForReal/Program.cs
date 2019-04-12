using IronPython.Hosting;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterceptTextChangeForReal
{
    class Program
    {
        static string workingFolder = @"C:\Users\Benedikt\Desktop\Bibelzitate Randomizer\6499 - Pokemon - Weisse Edition 2 (Germany) (NDSi Enhanced)_SDSME\";
        static DataGridView dataGridView6 = new DataGridView();
        static dynamic listlens;
        static dynamic wordslens;
        static dynamic capwordslens;
        static List<string> importantFormatChars = new List<string>()
        {
            @"\n",
            @"\r",

            //TODO: Try to fix them
            @"\xF000븁\x0000",
            @"\xF000븀\x0000",
       //     @"\c"
        };
        static string defaultText = "Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat Bibelzitat";

        static Random random = new Random();

        static void Main(string[] args)
        {
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");
            dataGridView6.Columns.Add("", "");

            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(@"C:\Users\Benedikt\Desktop\Bibelzitate Randomizer\dataDump.json"));

            listlens = jsonData.listlens;
            wordslens = jsonData.wordslens;
            capwordslens = jsonData.capwordslens;

            readTextV();
        }

        private static void readTextV() // Read V Text File
        {
           // int mainKey = 31881;
            bool compressed = false;
            foreach (string path in new string[] { "texts2", "texts" })
            {
                for (int ii = 0; ii < Directory.GetFiles(workingFolder + @"data\a\0\0\" + path + "\\").Length; ii++)
                {
                    int mainKey = 31881;
                    dataGridView6.Rows.Clear();

                    List<string> pokemonTexts = new List<string>();
                    System.IO.BinaryReader readText = new System.IO.BinaryReader(File.OpenRead(workingFolder + @"data\a\0\0\" + path + "\\" + ii.ToString("D4")));
                    int textSections = readText.ReadUInt16();
                    uint[] sectionOffset = new uint[3];
                    uint[] sectionSize = new uint[3];
                    int stringCount = readText.ReadUInt16();
                    int stringOffset;
                    int stringSize;
                    int[] stringUnknown = new int[3];
                    sectionSize[0] = readText.ReadUInt32();
                    int initialKey = (int)readText.ReadUInt32();
                    int key;
                    for (int i = 0; i < textSections; i++)
                    {
                        sectionOffset[i] = readText.ReadUInt32();
                    }

                    List<object[]> addables = new List<object[]>();

                    for (int j = 0; j < stringCount; j++)
                    {
                        #region Layer 1
                        readText.BaseStream.Position = sectionOffset[0];
                        sectionSize[0] = readText.ReadUInt32();
                        readText.BaseStream.Position += j * 8;
                        stringOffset = (int)readText.ReadUInt32();
                        stringSize = readText.ReadUInt16();
                        stringUnknown[0] = readText.ReadUInt16();
                        string pokemonText = "";
                        string pokemonText2 = "";
                        string pokemonText3 = "";
                        readText.BaseStream.Position = sectionOffset[0] + stringOffset;
                        key = mainKey;
                        for (int k = 0; k < stringSize; k++)
                        {
                            int car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                            if (compressed)
                            {
                                #region Compressed String
                                int shift = 0;
                                int trans = 0;
                                string uncomp = "";
                                while (true)
                                {
                                    int tmp = car >> shift;
                                    int tmp1 = tmp;
                                    if (shift >= 0x10)
                                    {
                                        shift -= 0x10;
                                        if (shift > 0)
                                        {
                                            tmp1 = (trans | ((car << (9 - shift)) & 0x1FF));
                                            if ((tmp1 & 0xFF) == 0xFF)
                                            {
                                                break;
                                            }
                                            if (tmp1 != 0x0 && tmp1 != 0x1)
                                            {
                                                uncomp += Convert.ToChar(tmp1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmp1 = ((car >> shift) & 0x1FF);
                                        if ((tmp1 & 0xFF) == 0xFF)
                                        {
                                            break;
                                        }
                                        if (tmp1 != 0x0 && tmp1 != 0x1)
                                        {
                                            uncomp += Convert.ToChar(tmp1);
                                        }
                                        shift += 9;
                                        if (shift < 0x10)
                                        {
                                            trans = ((car >> shift) & 0x1FF);
                                            shift += 9;
                                        }
                                        key = ((key << 3) | (key >> 13)) & 0xFFFF;
                                        car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                                        k++;
                                    }
                                }
                                #endregion
                                pokemonText += uncomp;
                            }
                            else if (car == 0xFFFF)
                            {
                            }
                            else if (car == 0xF100)
                            {
                                compressed = true;
                            }
                            else if (car == 0xFFFE)
                            {
                                pokemonText += @"\n";
                            }
                            else if (car > 20 && car <= 0xFFF0 && car != 0xF000 && Char.GetUnicodeCategory(Convert.ToChar(car)) != UnicodeCategory.OtherNotAssigned)
                            {
                                pokemonText += Convert.ToChar(car);
                            }
                            else
                            {
                                pokemonText += @"\x" + car.ToString("X4");
                            }
                            key = ((key << 3) | (key >> 13)) & 0xFFFF;
                        }
                        compressed = false;
                        #endregion
                        #region Layer 2
                        if (textSections > 1)
                        {
                            readText.BaseStream.Position = sectionOffset[1];
                            sectionSize[1] = readText.ReadUInt32();
                            readText.BaseStream.Position += j * 8;
                            stringOffset = (int)readText.ReadUInt32();
                            stringSize = readText.ReadUInt16();
                            stringUnknown[1] = readText.ReadUInt16();
                            readText.BaseStream.Position = sectionOffset[1] + stringOffset;
                            key = mainKey;
                            for (int k = 0; k < stringSize; k++)
                            {
                                int car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                                if (compressed)
                                {
                                    #region Compressed String
                                    int shift = 0;
                                    int trans = 0;
                                    string uncomp = "";
                                    while (true)
                                    {
                                        int tmp = car >> shift;
                                        int tmp1 = tmp;
                                        if (shift >= 0x10)
                                        {
                                            shift -= 0x10;
                                            if (shift > 0)
                                            {
                                                tmp1 = (trans | ((car << (9 - shift)) & 0x1FF));
                                                if ((tmp1 & 0xFF) == 0xFF)
                                                {
                                                    break;
                                                }
                                                if (tmp1 != 0x0 && tmp1 != 0x1)
                                                {
                                                    uncomp += Convert.ToChar(tmp1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tmp1 = ((car >> shift) & 0x1FF);
                                            if ((tmp1 & 0xFF) == 0xFF)
                                            {
                                                break;
                                            }
                                            if (tmp1 != 0x0 && tmp1 != 0x1)
                                            {
                                                uncomp += Convert.ToChar(tmp1);
                                            }
                                            shift += 9;
                                            if (shift < 0x10)
                                            {
                                                trans = ((car >> shift) & 0x1FF);
                                                shift += 9;
                                            }
                                            key = ((key << 3) | (key >> 13)) & 0xFFFF;
                                            car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                                            k++;
                                        }
                                    }
                                    #endregion
                                    pokemonText2 += uncomp;
                                }
                                else if (car == 0xFFFF)
                                {
                                }
                                else if (car == 0xF100)
                                {
                                    compressed = true;
                                }
                                else if (car == 0xFFFE)
                                {
                                    pokemonText2 += @"\n";
                                }
                                else if (car > 20 && car <= 0xFFF0 && car != 0xF000 && Char.GetUnicodeCategory(Convert.ToChar(car)) != UnicodeCategory.OtherNotAssigned)
                                {
                                    pokemonText2 += Convert.ToChar(car);
                                }
                                else
                                {
                                    pokemonText2 += @"\x" + car.ToString("X4");
                                }
                                key = ((key << 3) | (key >> 13)) & 0xFFFF;
                            }
                            compressed = false;
                        }
                        #endregion
                        #region Layer 3
                        if (textSections > 2)
                        {
                            readText.BaseStream.Position = sectionOffset[2];
                            sectionSize[2] = readText.ReadUInt32();
                            readText.BaseStream.Position += j * 8;
                            stringOffset = (int)readText.ReadUInt32();
                            stringSize = readText.ReadUInt16();
                            stringUnknown[2] = readText.ReadUInt16();
                            readText.BaseStream.Position = sectionOffset[2] + stringOffset;
                            key = mainKey;
                            for (int k = 0; k < stringSize; k++)
                            {
                                int car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                                if (compressed)
                                {
                                    #region Compressed String
                                    int shift = 0;
                                    int trans = 0;
                                    string uncomp = "";
                                    while (true)
                                    {
                                        int tmp = car >> shift;
                                        int tmp1 = tmp;
                                        if (shift >= 0x10)
                                        {
                                            shift -= 0x10;
                                            if (shift > 0)
                                            {
                                                tmp1 = (trans | ((car << (9 - shift)) & 0x1FF));
                                                if ((tmp1 & 0xFF) == 0xFF)
                                                {
                                                    break;
                                                }
                                                if (tmp1 != 0x0 && tmp1 != 0x1)
                                                {
                                                    uncomp += Convert.ToChar(tmp1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tmp1 = ((car >> shift) & 0x1FF);
                                            if ((tmp1 & 0xFF) == 0xFF)
                                            {
                                                break;
                                            }
                                            if (tmp1 != 0x0 && tmp1 != 0x1)
                                            {
                                                uncomp += Convert.ToChar(tmp1);
                                            }
                                            shift += 9;
                                            if (shift < 0x10)
                                            {
                                                trans = ((car >> shift) & 0x1FF);
                                                shift += 9;
                                            }
                                            key = ((key << 3) | (key >> 13)) & 0xFFFF;
                                            car = Convert.ToUInt16(readText.ReadUInt16() ^ key);
                                            k++;
                                        }
                                    }
                                    #endregion
                                    pokemonText3 += uncomp;
                                }
                                else if (car == 0xFFFF)
                                {
                                }
                                else if (car == 0xF100)
                                {
                                    compressed = true;
                                }
                                else if (car == 0xFFFE)
                                {
                                    pokemonText3 += @"\n";
                                }
                                else if (car > 20 && car <= 0xFFF0 && car != 0xF000 && Char.GetUnicodeCategory(Convert.ToChar(car)) != UnicodeCategory.OtherNotAssigned)
                                {
                                    pokemonText3 += Convert.ToChar(car);
                                }
                                else
                                {
                                    pokemonText3 += @"\x" + car.ToString("X4");
                                }
                                key = ((key << 3) | (key >> 13)) & 0xFFFF;
                            }
                            compressed = false;
                        }
                        #endregion
                        pokemonTexts.Add(pokemonText);

                        addables.Add(new object[] { "", pokemonText, stringUnknown[0], pokemonText2, stringUnknown[1], pokemonText3, stringUnknown[2] });

                        mainKey += 0x2983;
                        if (mainKey > 0xFFFF) mainKey -= 0x10000;
                    }
                    readText.Close();

                    // TODO: Edit the text
                    editTexts(pokemonTexts, addables);

                    saveTextV(textSections, stringCount, initialKey, ii, path);

                }
            }
        }

        private static void editTexts(List<string> pokemonTexts, List<object[]> addables)
        {
            // TODO: Edit the text
            int count = 0;

            List<string> copyList = pokemonTexts.Select(item => (string)item.Clone()).ToList();

            foreach (string line in pokemonTexts)
            {

                string filterLine = line.Trim();

                int formatChars = 0;

                string removalLine = filterLine;

                for (int i = 0; i < importantFormatChars.Count; i++)
                {
                    formatChars += Regex.Matches(filterLine, Regex.Escape(importantFormatChars[i]), RegexOptions.IgnoreCase).Count * importantFormatChars[i].Length;
                    removalLine = removalLine.Replace(importantFormatChars[i], "");
                }


                int wordsNumber = filterLine.Length - formatChars;

                List<string> useList = new List<string>() { "" };
                bool skipLine = false;

                if (DoesPropertyExist(capwordslens, wordsNumber + ""))
                {
                    useList = ((JArray)capwordslens[wordsNumber + ""]).ToList().Select((entry) => entry.ToObject<string>()).ToList();
                }
                else if (DoesPropertyExist(listlens, wordsNumber + ""))
                {
                    useList = ((JArray)listlens[wordsNumber + ""]).ToList().Select((entry) => entry.ToObject<string>()).ToList();
                }
                else if (DoesPropertyExist(wordslens, wordsNumber + ""))
                {
                    useList = ((JArray)wordslens[wordsNumber + ""]).ToList().Select((entry) => entry.ToObject<string>()).ToList();
                }
                else
                {
                    skipLine = true;
                }

             /*   MatchCollection matches = Regex.Matches(filterLine, "(\\\\x[A-Z0-9]{4}.{1}\\\\x[A-Z0-9]{4})", RegexOptions.IgnoreCase);

                int indexMover = 0;
                foreach(Match match in matches)
                {
                    filterLine = filterLine.Remove(match.Index - indexMover, match.Value.Length);
                    indexMover += match.Value.Length;
                }*/

                if (!removalLine.Contains("\\x") && !skipLine && removalLine.Length > 2 && removalLine.Trim() != "")
                {

                    int index = random.Next(useList.Count);

                    string randomInstance = useList[index];

                    string editedLine = "";

                    int pointer = 0;

                    while (importantFormatChars.Any(c => filterLine.Contains(c)))
                    {
                        int dist = 999999;

                        string character = importantFormatChars[0];

                        foreach (string i in importantFormatChars)
                        {

                            if (filterLine.IndexOf(i) != -1 && filterLine.IndexOf(i) < dist)
                            {
                                dist = filterLine.IndexOf(i);

                                character = i;
                            }
                        }

                        if (dist != 0)
                        {
                            editedLine += randomInstance.Substring(pointer, dist);

                            filterLine = filterLine.Substring(dist);
                        }

                        editedLine += character;

                        filterLine = filterLine.Substring(character.Length);

                        pointer = pointer + dist;
                    }

                    editedLine += randomInstance.Substring(pointer);


                    copyList[count] = editedLine;

                    copyList[count] = copyList[count].Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

               /*     foreach (Match match in matches)
                    {
                        copyList[count] = copyList[count].Insert(match.Index, match.Value);
                    }*/

                }

                addables[count][1] = copyList[count];

                dataGridView6.Rows.Add(addables[count]);
                dataGridView6.Rows[count].HeaderCell.Value = count.ToString();

                count = count + 1;

            }

        }

        private static bool DoesPropertyExist(dynamic settings, string name)
        {
            try
            {
                var x = settings[name];
                if (x != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }

        private static void saveTextV(int textSections, int stringCount, int initialKey, int ii, string path) // Save V Text File
        {
            int mainKey = 31881;
            BinaryWriter writeText = new BinaryWriter(File.Create(workingFolder + @"data\a\0\0\" + path + "\\" + ii.ToString("D4")));
            writeText.Write(Convert.ToUInt16(textSections));
            writeText.Write(Convert.ToUInt16(stringCount));
            int[] sectionSize = new int[3];
            int[] stringLength = new int[stringCount];
            int[] stringLength2 = new int[stringCount];
            int[] stringLength3 = new int[stringCount];
            int key;
            sectionSize[0] = 4 + (8 * stringCount);
            for (int i = 0; i < stringCount; i++)
            {
                stringLength[i] = getVStringLength(i, 1);
                sectionSize[0] += stringLength[i] * 2;
            }
            if (textSections > 1)
            {
                sectionSize[1] = 4 + (8 * stringCount);
                for (int i = 0; i < stringCount; i++)
                {
                    stringLength2[i] = getVStringLength(i, 3);
                    sectionSize[1] += stringLength2[i] * 2;
                }
                if (textSections > 2)
                {
                    sectionSize[2] = 4 + (8 * stringCount);
                    for (int i = 0; i < stringCount; i++)
                    {
                        stringLength3[i] = getVStringLength(i, 3);
                        sectionSize[2] += stringLength3[i] * 2;
                    }
                }
            }
            writeText.Write(sectionSize[0]);
            writeText.Write(initialKey);
            if (textSections == 1)
            {
                writeText.Write(0x10);
            }
            else if (textSections == 2)
            {
                writeText.Write(0x14);
                writeText.Write(0x14 + sectionSize[0]);
            }
            else
            {
                writeText.Write(0x18);
                writeText.Write(0x18 + sectionSize[0]);
                writeText.Write(0x18 + sectionSize[0] + sectionSize[1]);
            }

            #region Layer 1
            writeText.Write(sectionSize[0]);
            int offset = 4 + 8 * stringCount;
            for (int i = 0; i < stringCount; i++)
            {
                writeText.Write(offset);
                writeText.Write(Convert.ToUInt16(stringLength[i]));
                writeText.Write(Convert.ToUInt16(dataGridView6.Rows[i].Cells[2].Value));
                offset += stringLength[i] * 2;
            }
            for (int i = 0; i < stringCount; i++)
            {
                int[] currentString = EncodeVString(i, 1, stringLength[i]);
                key = mainKey;
                for (int j = 0; j < stringLength[i]; j++)
                {
                    if (j == stringLength[i] - 1)
                    {
                        writeText.Write(Convert.ToUInt16(key ^ 0xFFFF));
                        break;
                    }
                    writeText.Write(Convert.ToUInt16((currentString[j] ^ key) & 0xFFFF));
                    key = ((key << 3) | (key >> 13)) & 0xFFFF;
                }
                mainKey += 0x2983;
                if (mainKey > 0xFFFF) mainKey -= 0x10000;
            }
            #endregion
            #region Layer 2
            if (textSections > 1)
            {
                mainKey = 31881;
                writeText.Write(sectionSize[1]);
                offset = 4 + 8 * stringCount;
                for (int i = 0; i < stringCount; i++)
                {
                    writeText.Write(offset);
                    writeText.Write(Convert.ToUInt16(stringLength2[i]));
                    writeText.Write(Convert.ToUInt16(dataGridView6.Rows[i].Cells[4].Value));
                    offset += stringLength2[i] * 2;
                }
                for (int i = 0; i < stringCount; i++)
                {
                    int[] currentString = EncodeVString(i, 3, stringLength2[i]);
                    key = mainKey;
                    for (int j = 0; j < stringLength2[i]; j++)
                    {
                        if (j == stringLength2[i] - 1)
                        {
                            writeText.Write(Convert.ToUInt16(key ^ 0xFFFF));
                            break;
                        }
                        writeText.Write(Convert.ToUInt16((currentString[j] ^ key) & 0xFFFF));
                        key = ((key << 3) | (key >> 13)) & 0xFFFF;
                    }
                    mainKey += 0x2983;
                    if (mainKey > 0xFFFF) mainKey -= 0x10000;
                }
                #region Layer 3
                if (textSections > 2)
                {
                    mainKey = 31881;
                    writeText.Write(sectionSize[2]);
                    offset = 4 + 8 * stringCount;
                    for (int i = 0; i < stringCount; i++)
                    {
                        writeText.Write(offset);
                        writeText.Write(Convert.ToUInt16(stringLength3[i]));
                        writeText.Write(Convert.ToUInt16(dataGridView6.Rows[i].Cells[6].Value));
                        offset += stringLength3[i] * 2;
                    }
                    for (int i = 0; i < stringCount; i++)
                    {
                        int[] currentString = EncodeVString(i, 5, stringLength3[i]);
                        key = mainKey;
                        for (int j = 0; j < stringLength3[i]; j++)
                        {
                            if (j == stringLength3[i] - 1)
                            {
                                writeText.Write(Convert.ToUInt16(key ^ 0xFFFF));
                                break;
                            }
                            writeText.Write(Convert.ToUInt16((currentString[j] ^ key) & 0xFFFF));
                            key = ((key << 3) | (key >> 13)) & 0xFFFF;
                        }
                        mainKey += 0x2983;
                        if (mainKey > 0xFFFF) mainKey -= 0x10000;
                    }
                }
                #endregion
            }
            #endregion
            writeText.Close();

            /*
            #region Name List Updates
            if (comboBox3.SelectedIndex == 89 && radioButton14.Checked && isBW) // BW Place Names
            {
                int[] index = new int[dataGridView7.RowCount];
                for (int i = 0; i < dataGridView7.RowCount; i++)
                {
                    index[i] = nameText.IndexOf(dataGridView7.Rows[i].Cells[16].Value.ToString());
                }
                nameText.Clear();
                for (int i = 0; i < dataGridView6.RowCount; i++)
                {
                    nameText.Add(dataGridView6.Rows[i].Cells[1].Value.ToString());
                }
                for (int i = 0; i < dataGridView7.RowCount; i++)
                {
                    dataGridView7.Rows[i].Cells[16].Value = nameText[index[i]];
                }
            }
            if (comboBox3.SelectedIndex == 109 && radioButton14.Checked && isB2W2) // B2W2 Place Names
            {
                int[] index = new int[dataGridView7.RowCount];
                for (int i = 0; i < dataGridView7.RowCount; i++)
                {
                    index[i] = nameText.IndexOf(dataGridView7.Rows[i].Cells[16].Value.ToString());
                }
                nameText.Clear();
                for (int i = 0; i < dataGridView6.RowCount; i++)
                {
                    nameText.Add(dataGridView6.Rows[i].Cells[1].Value.ToString());
                }
                for (int i = 0; i < dataGridView7.RowCount; i++)
                {
                    dataGridView7.Rows[i].Cells[16].Value = nameText[index[i]];
                }
            }
            #endregion
            */
        }


        private static int[] EncodeVString(int stringIndex, int column, int stringSize) // Converts V string to hex characters
        {
            int[] pokemonMessage = new int[stringSize - 1];
            string currentMessage = "";
            try { currentMessage = dataGridView6[column, stringIndex].Value.ToString(); }
            catch { }
            var charArray = currentMessage.ToCharArray();
            int count = 0;
            for (int i = 0; i < currentMessage.Length; i++)
            {
                if (charArray[i] == '\\')
                {
                    if (charArray[i + 1] == 'n')
                    {
                        pokemonMessage[count] = 0xFFFE;
                        i++;
                    }
                    else
                    {
                        if (charArray[i + 1] == 'x')
                        {
                            string characterID = ((char)charArray[i + 2]).ToString() + ((char)charArray[i + 3]).ToString() + ((char)charArray[i + 4]).ToString() + ((char)charArray[i + 5]).ToString();
                            pokemonMessage[count] = (int)Convert.ToUInt32(characterID, 16);
                            i += 5;
                        }
                        else
                        {
                            pokemonMessage[count] = (int)charArray[i];
                        }
                    }
                }
                else
                {
                    pokemonMessage[count] = (int)charArray[i];
                }
                count++;
            }
            return pokemonMessage;
        }

        private static int getVStringLength(int stringIndex, int column) // Calculates V string length
        {
            int count = 0;
            string currentMessage = "";
            if (dataGridView6[column, stringIndex].Value == null)
            {
                return 1;
            }
            currentMessage = dataGridView6[column, stringIndex].Value.ToString();
            var charArray = currentMessage.ToCharArray();
            for (int i = 0; i < currentMessage.Length; i++)
            {
                if (charArray[i] == '\\')
                {
                    if (charArray[i + 1] == 'n')
                    {
                        count++;
                        i++;
                    }
                    else
                    {
                        if (charArray[i + 1] == 'x')
                        {
                            count++;
                            i += 5;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
                else
                {
                    count++;
                }
            }
            count++;
            return count;
        }

    }
}
