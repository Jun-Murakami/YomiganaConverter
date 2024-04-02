using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Dto;
using Avalonia.Controls;

namespace YomiganaConverter.ViewModels
{
    public partial class MainWindowViewModel
    { 

        private async Task RemoveLineBreaks()
        {

            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Confirmation",
                    ContentMessage = $"変換後テキストの改行を削除して繋げます。{Environment.NewLine}（空白行のみ残ります。）{Environment.NewLine}{Environment.NewLine}この操作は元に戻せません。実行しますか？",
                    ButtonDefinitions = ButtonEnum.OkCancel,
                    Icon = Icon.Question,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    ShowInCenter = true
                }
                );

            var result = await messageBoxStandardWindow.ShowWindowDialogAsync(GetWindow());
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
                        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
                            new MessageBoxStandardParams {
                                ContentTitle = "Confirmation",
                                ContentMessage = "[" + strA + "]と[" + strB + "]の合計数が前回の変換時と異なります。再変換しますか？",
                                ButtonDefinitions = ButtonEnum.OkCancel,
                                Icon = Icon.Question,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                CanResize = false,
                                ShowInCenter = true
                            });

                        var result = await messageBoxStandardWindow.ShowWindowDialogAsync(GetWindow());
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
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message + ex.StackTrace, ButtonEnum.Ok, Icon.Error);

                await messageBoxStandardWindow.ShowWindowDialogAsync(GetWindow());
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);

                return ;
            }
        }

    }
}

