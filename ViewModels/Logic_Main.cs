using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Text.RegularExpressions;

namespace YomiganaConverter.ViewModels
{
    public partial class MainWindowViewModel
    {
        private async Task MainGo()
        {
            if (string.IsNullOrEmpty(EditorText1))
            {
                return;
            }
            bool isYouon = false;
            bool isSokuon = false;
            List<string> outputData = new List<string>();
            int counter = 0;
            string lyricStr = EditorText1;
            string normalizedText = Regex.Replace(EditorText1, @"\r\n|\r|\n", Environment.NewLine);
            string[] lyricData = normalizedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            List<string> separateList = new List<string>();
            string currentLine = "";
            string currentChar = "";
            string keep = "";
            string keepKatakana = "";
            string keepEng = "";

            foreach (string target in lyricData)
            {
                foreach (char s in target)
                {
                    currentChar = s.ToString();
                    isYouon = false;
                    isSokuon = false;
                    if (Regex.IsMatch(currentChar, "[ャュョヮァィゥェォ]") && YouonIsChecked)
                    {
                        isYouon = true;
                    }
                    else if (currentChar == "ッ" && SokuonIsChecked)
                    {
                        isSokuon = true;
                    }
                    //カタカナの処理
                    if (Regex.IsMatch(currentChar, @"[ア-ヴｦ-ﾝ]") && KatakanaIsChecked)
                    {
                        if (keepKatakana == "")
                        {
                            keepKatakana = currentChar;
                            if (keepEng != "")
                            {
                                currentLine = currentLine + " " + keepEng;
                                keepEng = "";
                            }
                            else if (keep != "")
                            {
                                separateList.Add(keep);
                                if (isYouon || isSokuon)
                                {
                                    currentLine = currentLine + "Ka@" + counter.ToString() + "￥";
                                }
                                else
                                {
                                    currentLine = currentLine + " Ka@" + counter.ToString() + "￥";
                                }
                                counter++;
                                keep = "";
                            }
                        }
                        else
                        {
                            if (isYouon || isSokuon)
                            {
                                keepKatakana = keepKatakana + currentChar;
                            }
                            else
                            {
                                keepKatakana = keepKatakana + " " + currentChar;
                            }
                        }
                    }
                    //英語の処理
                    else if (Regex.IsMatch(currentChar, @"[A-Za-z]") && EnglishIsChecked)
                    {
                        if (keepEng == "")
                        {
                            keepEng = currentChar;
                            if (keepKatakana != "")
                            {
                                currentLine = currentLine + " " + keepKatakana;
                                keepKatakana = "";
                            }
                            else if (keep != "")
                            {
                                separateList.Add(keep);
                                if (isYouon || isSokuon)
                                {
                                    currentLine = currentLine + "Ka@" + counter.ToString() + "￥";
                                }
                                else
                                {
                                    currentLine = currentLine + " Ka@" + counter.ToString() + "￥";
                                }
                                counter++;
                                keep = "";
                            }
                        }
                        else
                        {
                            keepEng = keepEng + currentChar;
                        }
                    }
                    //ひらがなとその他の処理
                    else
                    {
                        if (keepKatakana != "")
                        {
                            currentLine = currentLine + " " + keepKatakana;
                            keep = keep + currentChar;
                            keepKatakana = "";
                        }
                        else if (keepEng != "")
                        {
                            currentLine = currentLine + " " + keepEng;
                            keep = keep + currentChar;
                            keepEng = "";
                        }
                        else
                        {
                            keep = keep + currentChar;
                        }
                    }
                }

                if (keepKatakana != "")
                {
                    currentLine = currentLine + " " + keepKatakana;
                    keepKatakana = "";
                    keep = "";
                }
                else if (keepEng != "")
                {
                    currentLine = currentLine + " " + keepEng;
                    keepEng = "";
                    keep = "";
                }
                else if (keep != "")
                {
                    separateList.Add(keep);
                    if (isYouon || isSokuon)
                    {
                        currentLine = currentLine + "Ka@" + counter.ToString() + "￥";
                    }
                    else
                    {
                        currentLine = currentLine + " Ka@" + counter.ToString() + "￥";
                    }
                    counter++;
                    keep = "";
                    keepKatakana = "";
                }
                else
                {
                    currentLine = currentLine + " " + currentChar;
                }

                //currentLine = currentLine.Substring(1);
                outputData.Add(currentLine);
                currentChar = "";
                currentLine = "";
            }

            //Debug.WriteLine("separateList : " + String.Join(",", separateList));
            //Debug.WriteLine("outputData : " + String.Join(",", outputData));

            string joinedList = String.Join("≒", separateList);
            joinedList = joinedList.Replace(',', '，');

            //Debug.WriteLine(joinedList);

            string result = await PostDataAsync(joinedList); // gooに接続

            result = Regex.Replace(result, " {2,}", " ");

            //Debug.WriteLine("result : " + result);

            String convertedData = "";

            foreach (char ch in result)
            {
                currentChar = ch.ToString();

                if (Regex.IsMatch(currentChar, "[ゃゅょゎぁぃぅぇぉャュョヮァィゥェォ]") && YouonIsChecked)
                {
                    isYouon = true;
                }
                else if ((currentChar == "っ" || currentChar == "ッ") && SokuonIsChecked)
                {
                    isSokuon = true;
                }
                else if (char.IsWhiteSpace(currentChar[0]))
                {
                    currentChar = "";
                }

                if (!string.IsNullOrEmpty(convertedData) && isYouon != true && isSokuon != true)
                {
                    convertedData = convertedData + " ";
                }

                if (string.IsNullOrEmpty(convertedData))
                {
                    convertedData = currentChar;
                }
                else
                {
                    convertedData = convertedData + currentChar;
                }

                isYouon = false;
                isSokuon = false;
            }

            convertedData = Regex.Replace(convertedData, " {2,}", " ");
            List<string> convertedList = convertedData.Split('≒').ToList();

            string outputDataStr = string.Join(Environment.NewLine, outputData);

            //Debug.WriteLine(string.Join(",", convertedList));

            int count = 0;
            List<string> tempData = new List<string>();
            foreach (string convertedLine in convertedList)
            {
                outputDataStr = outputDataStr.Replace($"Ka@{count}￥", convertedLine);
                count++;
            }
            //Debug.WriteLine(string.Join(Environment.NewLine, tempData));
            if (SpaceIsChecked)
            {
                outputDataStr = Regex.Replace(outputDataStr, " {2,}", " ");
                outputDataStr = Regex.Replace(outputDataStr, $"{Environment.NewLine} ", Environment.NewLine);
                outputDataStr = Regex.Replace(outputDataStr, $" {Environment.NewLine}", Environment.NewLine);
                outputDataStr = Regex.Replace(outputDataStr, $"^[ ]+", "", RegexOptions.Singleline);
            }
            else
            {
                outputDataStr = outputDataStr.Replace(" ", "");
            }

            outputDataStr = outputDataStr.Replace("，", ",");
            outputDataStr = Regex.Replace(outputDataStr, @" ([.,;:/．，；：／@、。!?\""#$%&'=￥”＃＄％＆！？’＝｜])", @"$1");

            EditorText2 = outputDataStr;
        }

        async Task<string> PostDataAsync(string joinedList)
        {
            string URL = "https://labs.goo.ne.jp/api/hiragana";
            string appid = "92459f8ae678689f6a463f553c77c7cb3c36e67f8dbffc5d7c8f22c5412e23ea";
            string type = "hiragana";
            var requestData = new { app_id = appid, sentence = joinedList, output_type = type };
            var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            try
            {
                using var httpClient = new HttpClient();
                using var response = await httpClient.PostAsync(URL, requestContent);

                if (response.Content.Headers.ContentType!.MediaType == "application/json")
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<JsonElement>(contentStream);
                    //var jsonString = JsonSerializer.Serialize(responseJson);
                    return responseJson.GetProperty("converted").GetString()!.Trim(); ;
                }
                else
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Error", "APIから正常に文字列を取得できませんでした。Rate limit exceededというエラーが表示されている場合は、大変申し訳ありませんが本日のAPI利用上限を超えております。時間をおいてお試しください。" + ex.Message, ButtonEnum.Ok, Icon.Error);

                await messageBoxStandardWindow.ShowWindowDialogAsync(GetWindow());
                return "";
            }
        }
    }
}
