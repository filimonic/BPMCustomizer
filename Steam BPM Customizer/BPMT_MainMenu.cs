using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

#if _DEVELOPEMENT_MODE_

namespace Steam_BPM_Customizer
{
    class BPMT_MainMenu
    {
        public static byte[] Transform(byte[] dataIn, SteamItemSettings[] _unused, string steamPath)
        {
            SteamItemSettings[] settingList = BPM_OptionsManager.Getettings().ToArray();
            //Translate data to XML:
            Log(Properties.Resources.REPORT_TRANSFORMER_ALL_TRANSFORMING_BEGIN);
            XmlDocument xmlData;
            try
            {
                xmlData = new XmlDocument();
                xmlData.PreserveWhitespace = true;
                string xmlText = Encoding.UTF8.GetString(dataIn);
                xmlData.LoadXml(xmlText);
            }
            catch (Exception e)
            {
                Log(String.Format(Properties.Resources.ERROR_TRANSFORMER_ALL_ERROR_XML, e.Message));
                return null;
            }
            //return Encoding.UTF8.GetBytes(xmlData.OuterXml);
            foreach (SteamItemSettings setting in settingList)
            {
                //Log(String.Format("Processing setting {0}",setting.ToString("G")));
                ProcessSetting(xmlData, setting, steamPath);
            }
            RecalculateSettingsExitRow(xmlData);
            RecalculateMainMenuRow(xmlData, settingList);
            Log(Properties.Resources.REPORT_TRANSFORMER_ALL_TRANSFORMING_END);
            return Encoding.UTF8.GetBytes(xmlData.OuterXml);
        }
        private static void RecalculateSettingsExitRow(XmlDocument document)
        {
            int nodeCount = 1;
            //XmlNodeList buttonNodeList = document.SelectNodes(@"//Panel[@id='MenuRowButtons']/*Button|//Panel[@id='MenuRowButtons']/ParentalButton");
            XmlNodeList buttonNodeList = document.SelectNodes(@"//Panel[@id='SettingsExitRow']/*[self::Button or self::ParentalButton]");
            int maxNodeCount = buttonNodeList.Count;
            //Log("Recalculating nodes  total {" + maxNodeCount + "}");
            foreach (XmlNode buttonNode in buttonNodeList)
            {
                //Log("Recalculating node {" + nodeCount + "} { " + buttonNode.Name + buttonNode.OuterXml+ " }");
                string selectionPos = "";
                if (nodeCount == 1)
                {
                    selectionPos = "10." + (nodeCount * 2).ToString() +",0";
                }
                else if (nodeCount == maxNodeCount)
                {
                    selectionPos = "10." + (nodeCount + 1).ToString() + ",0";
                }
                else
                {
                    selectionPos = "10." + (nodeCount + 1).ToString() + ",0";
                }
                SetAttribute(buttonNode, @"tabindex", nodeCount.ToString());
                SetAttribute(buttonNode, @"selectionpos", selectionPos);
                nodeCount++;
            }
            //Log("Recalculating done");
        } 
        private static void RecalculateMainMenuRow(XmlDocument document, SteamItemSettings[] settings)
        {
            int nodeCount = 1;
            //XmlNodeList buttonNodeList = document.SelectNodes(@"//Panel[@id='MenuRowButtons']/*Button|//Panel[@id='MenuRowButtons']/ParentalButton");
            XmlNodeList buttonNodeList = document.SelectNodes(@"//Panel[@id='MenuRowButtons']/*[self::Button or self::ParentalButton]");
            int maxNodeCount = buttonNodeList.Count;
            //Log("Recalculating nodes  total {" + maxNodeCount + "}");

            string defaultButtonId = null;
            bool needChangeDefaultButtonId = (settings.Contains(SteamItemSettings.NoStoreButtonMainMenu) || settings.Contains(SteamItemSettings.GoToLibraryOnStart));

            foreach (XmlNode buttonNode in buttonNodeList)
            {
                //Log("Recalculating node {" + nodeCount + "} { " + buttonNode.Name + buttonNode.OuterXml+ " }");
                string selectionPos = "";
                string buttonClass = "";

                
                //Detect MainMenuRow first button not hidden
                if ((defaultButtonId == null) && (needChangeDefaultButtonId == true))
                {
                    if (buttonNode.Attributes[@"id"] != null)
                    {
                        if (buttonNode.Attributes[@"BPMT_visible"] != null)
                        {
                            if (buttonNode.Attributes[@"BPMT_visible"].Value != "0")
                            {
                                defaultButtonId = buttonNode.Attributes[@"id"].Value;
                            }
                        } 
                        else 
                        {
                            defaultButtonId = buttonNode.Attributes[@"id"].Value;
                        }
                    }
                }

                if (nodeCount == 1)
                {
                    selectionPos =  "1," + maxNodeCount.ToString();
                    buttonClass = "Button ButtonLeft";
                }
                else if (nodeCount == maxNodeCount )
                {
                    buttonClass = "Button ButtonRight";
                    selectionPos = "1." + nodeCount.ToString() + "," + maxNodeCount.ToString();
                } else {
                    buttonClass = "Button ButtonMiddle";
                    selectionPos =  "1." + nodeCount.ToString() + "," + maxNodeCount.ToString();
                }
                SetAttribute(buttonNode, @"tabindex", nodeCount.ToString());
                SetAttribute(buttonNode, @"class", buttonClass);
                SetAttribute(buttonNode, @"selectionpos", selectionPos);
                nodeCount++;
            }

            if (settings.Contains(SteamItemSettings.GoToLibraryOnStart))
            {
                defaultButtonId = "BPMT_HiddenGoToLibrary_Button";
            }

            //Set MainMenuRow first button
            if ((defaultButtonId != null) && (needChangeDefaultButtonId == true))
            {

                XmlNode mainMenuScreenNode = document.SelectSingleNode(@"//MainMenu");
                SetAttribute(mainMenuScreenNode, @"defaultfocus", defaultButtonId); //This does not works somewhy, but will leave it.
                XmlNode storeButton = document.SelectSingleNode(@"//Panel[@id='MenuRowButtons']/*[(self::Button or self::ParentalButton) and @id='StoreButton']");
                SetAttribute(storeButton, @"onfocus", string.Format("SetInputFocus( {0} )", defaultButtonId));
            }


        }

        private static bool ProcessSetting(XmlDocument document, SteamItemSettings setting, string steamPath)
        {
            switch (setting)
            {
                case SteamItemSettings.NoBrowserNavigation:
                     DisableNodeById(document, @"ProfileWrapper", @"ProfileWrapper");
                     DisableNodeById(document, @"ParentalButton", @"ProfileButton");
                     DisableNodeById(document, @"Panel", @"Avatar");
                     break;
                case SteamItemSettings.NoProfileIcon :
                     DisableNodeById(document, @"Button", @"Avatar") ;
                     //DisableNodeById(document, @"Panel", @"ProfileNameWrapper");
                     break;
                case SteamItemSettings.HomeScreenHideProfileName:
                    DisableNodeById(document, @"Label", @"ProfileNameLabel");
                    break;
                case SteamItemSettings.NoSettingsMainMenu:
                     DisableNodeById(document, @"ParentalButton", @"SettingsButton");
                     break;
                case SteamItemSettings.NoParentalLockButtonMainMenu:
                     DisableNodeById(document, @"ParentalButton", @"ParentalLockButton");
                     break;
                case SteamItemSettings.NoOnline:
                     DisableNodeById(document, @"Button", @"GoOnlineButton");
                     break;
                case SteamItemSettings.NoUpperExitMainMenu:
                     DisableNodeById(document, @"Button", @"Exit");
                     break;
                case SteamItemSettings.NoCommunityMainScreen:
                     DisableNodeById(document, @"ParentalButton", @"CommunityButton");
                     break;
                case SteamItemSettings.NoStoreButtonMainMenu:
                     DisableNodeById(document, @"ParentalButton", @"StoreButton");
                     break;
                case SteamItemSettings.AddQuitEntryMainMenu:
                case SteamItemSettings.AddSleepEntryMainMenu:
                case SteamItemSettings.AddShutdownEntryMainMenu:
                case SteamItemSettings.AddRestartEntryMainMenu:
                case SteamItemSettings.GoToLibraryOnStart:
                    AddMenuRowButton(document, setting);
                     break;
                case SteamItemSettings.AddUpperStoreMainMenu:
                     try
                     {
                         AddMenuUpperButton(document, setting, steamPath);
                     }
                     catch (Exception e)
                     {
                         Log("Error: " + e.Message);
                     }
                     
                     break;
                default:
                    return false;
            }
            return true;
        }

        private static void AddMenuUpperButton(XmlDocument document, SteamItemSettings setting, string steamPath)
        {
            XmlNode settingsExitRowNode = document.SelectSingleNode(@"//Panel[@id='SettingsExitRow']");
            XmlNode newParentalButton;
            XmlNode newParentalButtonLabel;
            XmlNode newParentalButtonImage;
            switch (setting)
            {
                case SteamItemSettings.AddUpperStoreMainMenu :
                    string shoppingCartImagePath = Path.Combine(steamPath, @"tenfoot\resource\images\BPMT_ShoppingCartIcon.png");
                    try //Store file
                    {
                        
                        System.IO.File.WriteAllBytes(shoppingCartImagePath, Properties.Resources.FILE_BPMT_ShoppingCartIcon);
                    }
                    catch (Exception e)
                    {
                        Log(String.Format(Properties.Resources.ERROR_TRANSFORMER_MAINMENU_UNABLE_TO_SAVE_FILE, shoppingCartImagePath, e.Message));
                    }
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButtonImage = document.CreateElement(@"Image");
                    settingsExitRowNode.AppendChild(newParentalButton);
                    newParentalButton.AppendChild(newParentalButtonImage);
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    SetAttribute(newParentalButton, @"id", @"BPMT_MainMenuUpperStoreButton");
                    SetAttribute(newParentalButton, @"onactivate", @"StoreShow();");
                    SetAttribute(newParentalButton, @"class", @"display: block;");
                    SetAttribute(newParentalButton, @"onfocus", @"ExitButtonFocused();");
                    SetAttribute(newParentalButtonLabel, @"style", @"text-transform: uppercase; margin-left: 46px;");
                    SetAttribute(newParentalButtonImage,@"src",@"file://{images}/BPMT_ShoppingCartIcon.png");
                    SetAttribute(newParentalButtonLabel, @"class", @"LongDesc");
                    SetAttribute(newParentalButtonLabel, @"text", @"#MainMenu_Store");
                    break;
            }
        }

        private static void SetAttribute(XmlNode node, string attributeName, string attributeValue)
        {
            if (node.Attributes.GetNamedItem(attributeName) != null)
            {
                node.Attributes.RemoveNamedItem(attributeName);
            }
            XmlAttribute attributeNode = node.OwnerDocument.CreateAttribute(attributeName);
            attributeNode.Value = attributeValue;
            node.Attributes.Append(attributeNode);
        }

        private static bool AddMenuRowButton(XmlDocument document, SteamItemSettings setting)
        {
            XmlNode menuRowNode = document.SelectSingleNode(@"//Panel[@id='MenuRowButtons']");
            XmlNode newParentalButton;
            XmlNode newParentalButtonLabel;
            switch (setting)
            {
                case SteamItemSettings.AddQuitEntryMainMenu :
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    menuRowNode.AppendChild(newParentalButton);
                    SetAttribute(newParentalButtonLabel, @"id", @"BPMT_ExitSteamMainMenu_Label");
                    SetAttribute(newParentalButtonLabel, @"text", @"#MainMenu_Exit");
                    SetAttribute(newParentalButtonLabel, @"class", @"ButtonLabel");
                    SetAttribute(newParentalButton, @"onactivate", @"ExitSteam()");
                    SetAttribute(newParentalButton, @"id", @"BPMT_ExitSteamMainMenu_Button");
                    break;
                case SteamItemSettings.AddSleepEntryMainMenu:
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    menuRowNode.AppendChild(newParentalButton);
                    SetAttribute(newParentalButtonLabel, @"id", @"BPMT_SleepComputerMainMenu_Label");
                    SetAttribute(newParentalButtonLabel, @"text", @"#Quit_Sleep");
                    SetAttribute(newParentalButtonLabel, @"class", @"ButtonLabel");
                    SetAttribute(newParentalButtonLabel, @"style", @"text-transform: uppercase;");
                    SetAttribute(newParentalButton, @"onactivate", @"CloseModalDialog(); AsyncEvent( 0.5, SuspendMachine() )");
                    SetAttribute(newParentalButton, @"id", @"BPMT_SleepComputerMainMenu_Button");
                    break;
                case SteamItemSettings.AddShutdownEntryMainMenu:
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    menuRowNode.AppendChild(newParentalButton);
                    SetAttribute(newParentalButtonLabel, @"id", @"BPMT_ShutdownComputerMainMenu_Label");
                    SetAttribute(newParentalButtonLabel, @"text", @"#Quit_Shutdown");
                    SetAttribute(newParentalButtonLabel, @"class", @"ButtonLabel");
                    SetAttribute(newParentalButtonLabel, @"style", @"text-transform: uppercase;");
                    SetAttribute(newParentalButton, @"onactivate", @"CloseModalDialog(); AsyncEvent( 0.5, ShutdownMachine() )");
                    SetAttribute(newParentalButton, @"id", @"BPMT_ShutdownComputerMainMenu_Button");
                    break;
                case SteamItemSettings.AddRestartEntryMainMenu:
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    menuRowNode.AppendChild(newParentalButton);
                    SetAttribute(newParentalButtonLabel, @"id", @"BPMT_RestartComputerMainMenu_Label");
                    SetAttribute(newParentalButtonLabel, @"text", @"#Quit_Restart");
                    SetAttribute(newParentalButtonLabel, @"class", @"ButtonLabel");
                    SetAttribute(newParentalButtonLabel, @"style", @"text-transform: uppercase;");
                    SetAttribute(newParentalButton, @"onactivate", @"CloseModalDialog(); AsyncEvent( 0.5, RestartMachine() )");
                    SetAttribute(newParentalButton, @"id", @"BPMT_RestartComputerMainMenu_Button");
                    break;
                case SteamItemSettings.GoToLibraryOnStart:
                    newParentalButton = document.CreateElement(@"Button");
                    newParentalButtonLabel = document.CreateElement(@"Label");
                    newParentalButton.AppendChild(newParentalButtonLabel);
                    menuRowNode.AppendChild(newParentalButton);
                    SetAttribute(newParentalButtonLabel, @"id", @"BPMT_HiddenGoToLibrary_Label");
                    SetAttribute(newParentalButtonLabel, @"text", @"BPM:HIDDEN_GO_TO_LIBRARY");
                    SetAttribute(newParentalButtonLabel, @"class", @"ButtonLabel");
                    SetAttribute(newParentalButton, @"style", @"visibility: collapse;");
                    SetAttribute(newParentalButton, @"onfocus", @"$.DispatchEvent( 'ShowLibrary' );");
                    SetAttribute(newParentalButton, @"id", @"BPMT_HiddenGoToLibrary_Button");
                    break;
            }
            
            return true;
        }

        private static bool DisableNodeById(XmlDocument document, string elementType, string elementId)
        {
            XmlNode node = document.SelectSingleNode(@"//" + elementType + "[@id='" + elementId + "']");
            if (node == null)
            {
                //Log("No such node found");
                return false;
            }
            SetAttribute(node, "style", "visibility: collapse;");
            //SetAttribute(node, "BPMT_visible", "0");
            Log(String.Format(Properties.Resources.REPORT_TRANSFORMER_MAINMENU_NODE_DISABLED,elementId));
            return true;
        }


        private static void Log(string message)
        {
            BPM_Log.Log(Properties.Resources.REPORT_TRANSFORMER_MAINMENU_LOGPREFIX + message);
        }
    }
}
#endif