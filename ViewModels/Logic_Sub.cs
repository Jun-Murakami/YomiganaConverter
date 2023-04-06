using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input.Platform;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using System.Diagnostics;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;
using System.Linq;
using MessageBoxAvaloniaEnums = MessageBox.Avalonia.Enums;

using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq.Expressions;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using MessageBox.Avalonia.Enums;

namespace YomiganaConverter.ViewModels
{
    public partial class MainWindowViewModel
    { 

        private async Task RemoveLineBreaks()
        {

            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Confirmation", $"変換後テキストの改行を削除して繋げます。{Environment.NewLine}（空白行のみ残ります。）{Environment.NewLine}{Environment.NewLine}この操作は元に戻せません。実行しますか？", MessageBoxAvaloniaEnums.ButtonEnum.OkCancel, MessageBoxAvaloniaEnums.Icon.Question);

            var result = await messageBoxStandardWindow.ShowDialog(GetWindow());
            if (result == ButtonResult.Ok)
            {
                if (string.IsNullOrEmpty(EditorText2))
                {
                    return;
                }
                string normalizedText = Regex.Replace(EditorText2, @"\r\n|\r|\n", Environment.NewLine);
                string[] convertedData = normalizedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                List<string> outputList = new List<string> {};
                string outputData = "";
                bool isConsective = false;

                Debug.WriteLine(convertedData.Count());

                foreach (string line in convertedData)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (!isConsective)
                        {
                            outputData += "\t";
                        }
                        isConsective = true;
                    }
                    else
                    {
                        outputData += line;
                        isConsective = false;
                    }
                }

                //outputData = string.Join(Environment.NewLine, outputList);
                //Debug.WriteLine(outputData);
                outputData = outputData.Replace("\t", Environment.NewLine + Environment.NewLine);

                EditorText2 = outputData;
            }
        }

        //グローバル変数
        public static Dictionary<string, string> convertSwitch = new Dictionary<string, string>{
                    {"は", ""},
                    {"へ", ""}
                };
        public static List<string> convertStockHaWa = new List<string>();
        public static List<string> convertStockHeE = new List<string>();

        //strAとstrBを相互変換するメソッド
        public async Task Convert(string strA, string strB)
        {
            if (string.IsNullOrEmpty(EditorText2))
            {
                return;
            }
            List<string> convertList = new List<string>();
            string normalizedText = Regex.Replace(EditorText2, @"\r\n|\r|\n", Environment.NewLine);
            string convertedLyric = normalizedText;
            List<string> outputList = new List<string>();
            string outputDataStr = "";
            List<string> convertStock = new List<string>();

            try
            {
                if (strA == "は")
                {
                    convertStock = convertStockHaWa;
                }
                else
                {
                    convertStock = convertStockHeE;
                }
                if (convertSwitch[strA] == "")
                {
                    foreach (char c in convertedLyric)
                    {
                        string strC = c.ToString();
                        if (strC == strA)
                        {
                            convertList.Add(strB);
                            convertStock.Add(strA);
                        }
                        else if (strC == strB)
                        {
                            convertList.Add(strB);
                            convertStock.Add(strB);
                        }
                    }
                    convertSwitch[strA] = strA;
                }
                else
                {
                    foreach (char c in convertedLyric)
                    {
                        string strC = c.ToString();
                        if (strC == strA)
                        {
                            if (convertSwitch[strA] == strB)
                            {
                                convertList.Add(strB);
                            }
                            else
                            {
                                convertList.Add(strA);
                            }
                        }
                        else if (strC == strB)
                        {
                            if (convertSwitch[strA] == strA)
                            {
                                convertList.Add(strA);
                            }
                            else
                            {
                                convertList.Add(strB);
                            }
                        }
                    }
                    if (convertList.Count != convertStock.Count)
                    {
                        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Confirmation", "[" + strA + "]と[" + strB + "]の合計数が前回の変換時と異なります。再変換しますか？", MessageBoxAvaloniaEnums.ButtonEnum.OkCancel,MessageBoxAvaloniaEnums.Icon.Question);

                        var result = await messageBoxStandardWindow.ShowDialog(GetWindow());
                        if (result != ButtonResult.Ok)
                        {
                            return;
                        }
                        else
                        {
                            convertStock.Clear();
                            foreach (char c in convertedLyric)
                            {
                                string strC = c.ToString();
                                if (strC == strA)
                                {
                                    convertStock.Add(strA);
                                }
                                else if (strC == strB)
                                {
                                    convertStock.Add(strB);
                                }
                            }
                        }
                    }
                    int counter = 0;
                    List<string> listTemp = new List<string>(convertList);
                    foreach (string s in convertList)
                    {
                        if (s == strA)
                        {
                            if (convertStock[counter] == strB && convertSwitch[strA] == strA)
                            {
                                listTemp[counter] = strB;
                            }
                        }
                        counter++;
                    }
                    convertList = listTemp;
                    if (convertSwitch[strA] == strB)
                    {
                        convertSwitch[strA] = strA;
                    }
                    else
                    {
                        convertSwitch[strA] = strB;
                    }
                }
                int counterA = 0;
                int counterB = 0;
                foreach (char c in convertedLyric)
                {
                    string strC = c.ToString();
                    if (strC == strA || strC == strB)
                    {
                        outputList.Add(convertList[counterB]);
                        counterB++;
                    }
                    else if (strC == "")
                    {
                        outputList.Add("\t");
                    }
                    else
                    {
                        outputList.Add(strC);
                    }
                    counterA++;
                }

                outputDataStr = string.Join("", outputList);
                outputDataStr = outputDataStr.Replace("\t", Environment.NewLine);

                EditorText2 = outputDataStr;

                if (strA == "は")
                {
                    convertStockHaWa = convertStock;
                    if (convertSwitch[strA] == strA)
                    {
                        ButtonTextHawa = "[" + strB + "]→" + strA;
                    }
                    else
                    {
                        ButtonTextHawa = "[" + strA + "]→" + strB;
                    }
                }
                else
                {
                    convertStockHeE = convertStock;
                    if (convertSwitch[strA] == strA)
                    {
                        ButtonTextHeE = "[" + strB + "]→" + strA;
                    }
                    else
                    {
                        ButtonTextHeE = "[" + strA + "]→" + strB;
                    }
                }
            }
            catch (Exception ex)
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message + ex.StackTrace, MessageBoxAvaloniaEnums.ButtonEnum.Ok, MessageBoxAvaloniaEnums.Icon.Error);

                await messageBoxStandardWindow.ShowDialog(GetWindow());
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);

                return ;
            }
        }

    }
}

