using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using ProjectEye.Core.Models.Options;
using ProjectEye.Models.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using static ProjectEye.Core.WindowManager;

namespace ProjectEye.Core.Service
{
    public class ThemeService : IService
    {
        private readonly ConfigService config;
        private readonly SystemResourcesService systemResources;
        private readonly Theme theme;


        public delegate void ThemeChangedEventHandler(string OldThemeName, string NewThemeName);
        /// <summary>
        /// 当切换主题时发生
        /// </summary>
        public event ThemeChangedEventHandler OnChangedTheme;
        public ThemeService(ConfigService config,
            SystemResourcesService systemResources)
        {
            this.config = config;
            this.systemResources = systemResources;
            theme = new Theme();
        }
        public void Init()
        {
            string themeName = config.options.Style.Theme.ThemeName;
            if (systemResources.Themes.Where(m => m.ThemeName == themeName).Count() == 0)
            {
                themeName = systemResources.Themes[0].ThemeName;
                config.options.Style.Theme = systemResources.Themes[0];
                //config.Save();
            }
            Project1.UI.Cores.UIDefaultSetting.DefaultThemeName = themeName;

            Project1.UI.Cores.UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";

            HandleDarkMode();
        }
        /// <summary>
        /// 设置主题
        /// </summary>
        /// <param name="themeName"></param>
        public void SetTheme(string themeName)
        {

            if (Project1.UI.Cores.UIDefaultSetting.DefaultThemeName != themeName)
            {
                string oldName = Project1.UI.Cores.UIDefaultSetting.DefaultThemeName;

                Project1.UI.Cores.UIDefaultSetting.DefaultThemeName = themeName;

                Project1.UI.Cores.UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";

                theme.ApplyTheme();

                OnChangedTheme?.Invoke(oldName, themeName);
            }
        }

        public void HandleDarkMode()
        {
            string darkModeThemeName = "Dark";
            if (config.options.Style.IsAutoDarkMode)
            {
                var darkTheme = systemResources.Themes.Where(m => m.ThemeName == darkModeThemeName).FirstOrDefault();
                if (darkTheme == null)
                {
                    return;
                }
                DateTime startTime = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    config.options.Style.AutoDarkStartH,
                   config.options.Style.AutoDarkStartM,
                    0);
                DateTime endTime = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    config.options.Style.AutoDarkEndH,
                   config.options.Style.AutoDarkEndM,
                    0);

                bool isOpen = false;

                if (config.options.Style.AutoDarkStartH <= config.options.Style.AutoDarkEndH)
                {
                    isOpen = DateTime.Now >= startTime && DateTime.Now <= endTime;
                }
                else
                {
                    isOpen = DateTime.Now >= startTime || DateTime.Now <= endTime;
                }
                if (isOpen)
                {
                    if (config.options.Style.Theme != darkTheme)
                    {
                        Debug.WriteLine("dark mode open!");
                        config.options.Style.Theme = darkTheme;

                        SetTheme(darkModeThemeName);

                    }
                }
                else
                {
                    var defualtTheme = systemResources.Themes[0];
                    if (config.options.Style.Theme != defualtTheme)
                    {
                        Debug.WriteLine("dark mode close!");
                        config.options.Style.Theme = defualtTheme;

                        SetTheme(defualtTheme.ThemeName);

                    }
                }
            }
        }

        /// <summary>
        /// 创建默认的提示界面布局UI
        /// </summary>
        /// <param name="themeName">主题名</param>
        /// <param name="screenName">屏幕名称</param>
        /// <returns></returns>
        public UIDesignModel GetCreateDefaultTipWindowUI(
            string themeName,
            string screenName,
            Position position
            )
        {
            if(position == Position.Design)
            {
                position = Position.Full;
            }
            screenName = screenName.Replace("\\", "");

            PositionModel positionModel = GetPositonInfo(position);
            Size screenSize;
            if (position == Position.Full)
            {
                var screen = System.Windows.Forms.Screen.PrimaryScreen;
                if (screenName != string.Empty)
                {
                    foreach (var item in System.Windows.Forms.Screen.AllScreens)
                    {
                        string itemScreenName = item.DeviceName.Replace("\\", "");
                        if (itemScreenName == screenName)
                        {
                            screen = item;
                            break;
                        }
                    }
                }

                screenSize = WindowManager.GetSize(screen);
                //double imgWidth = 272;
                //double imgHeight = 187;
                //if (windowWidth > 0 && windowHeight > 0)
                //{
                //    screenSize.Width = windowWidth;
                //    screenSize.Height = windowHeight;
                //    imgWidth = windowWidth / 5;
                //    imgHeight = windowHeight / 5;
                //}
            }
            else
            {
                screenSize = new Size();
                screenSize.Width = positionModel.WindowsSize.Width;
                screenSize.Height = positionModel.WindowsSize.Height;
            }

            //创建默认布局
            var data = new UIDesignModel();
            data.ContainerAttr = new ContainerModel()
            {
                Background = Brushes.White,
                Opacity = .98
            };

            var elements = new List<ElementModel>();
            var tipImage = new ElementModel();
            tipImage.Type = Project1.UI.Controls.Enums.DesignItemType.Image;
            tipImage.Width = positionModel.TipImage.Size.Width;//imgWidth;
            tipImage.Opacity = 1;
            tipImage.Height = positionModel.TipImage.Size.Height;//imgHeight;
            tipImage.Image = $"pack://application:,,,/ProjectEye;component/Resources/Themes/{themeName}/Images/tipImage.png";
            tipImage.X = positionModel.TipImage.X;
            tipImage.Y = positionModel.TipImage.Y;

            var tipText = new ElementModel();
            tipText.Type = Project1.UI.Controls.Enums.DesignItemType.Text;
            tipText.Text = "您已持续用眼{t}分钟，休息一会吧！请将注意力集中在至少6米远的地方20秒！";
            tipText.Opacity = 1;
            tipText.TextColor = Project1UIColor.Get("#45435b");
            tipText.Width = positionModel.TipText.Size.Width;//400;
            tipText.Height = positionModel.TipText.Size.Height; //50;
            tipText.FontSize = positionModel.TipText.FontSize;
            tipText.X = positionModel.TipText.X;
            tipText.Y = positionModel.TipText.Y;
            

            var restBtn = new ElementModel();
            restBtn.Type = Project1.UI.Controls.Enums.DesignItemType.Button;
            restBtn.Width = positionModel.RestBtn.Size.Width;//110;
            restBtn.Height = positionModel.RestBtn.Size.Height;//45;
            restBtn.FontSize = positionModel.RestBtn.FontSize;//14;
            restBtn.Text = "好的";
            restBtn.Opacity = 1;
            restBtn.Command = "rest";
            restBtn.X = positionModel.RestBtn.X;
            restBtn.Y = positionModel.RestBtn.Y;

            var breakBtn = new ElementModel();
            breakBtn.Type = Project1.UI.Controls.Enums.DesignItemType.Button;
            breakBtn.Width = positionModel.BreakBtn.Size.Width;//110;
            breakBtn.Height = positionModel.BreakBtn.Size.Height;//45;
            breakBtn.FontSize = positionModel.BreakBtn.FontSize;//14;
            breakBtn.Text = "暂时不";
            breakBtn.Style = "basic";
            breakBtn.Command = "break";
            breakBtn.Opacity = 1;
            breakBtn.X = positionModel.BreakBtn.X;
            breakBtn.Y = positionModel.BreakBtn.Y;

            var countDownText = new ElementModel();
            countDownText.Text = "{countdown}";
            countDownText.FontSize = positionModel.CountDownText.FontSize;//50;
            countDownText.IsTextBold = true;
            countDownText.Type = Project1.UI.Controls.Enums.DesignItemType.Text;
            countDownText.TextColor = Brushes.Black;
            countDownText.Opacity = 1;
            countDownText.Width = positionModel.CountDownText.Size.Width;//100;
            countDownText.Height = positionModel.CountDownText.Size.Height;//60;
            countDownText.X = positionModel.CountDownText.X;
            countDownText.Y = positionModel.CountDownText.Y;

            if (themeName == "Dark")
            {
                //深色主题的样式

                data.ContainerAttr.Background = Project1UIColor.Get("#1A1B1C");
                tipText.TextColor = Project1UIColor.Get("#D9D9D9");
                countDownText.TextColor = Project1UIColor.Get("#D9D9D9");

            }
            elements.Add(tipImage);
            elements.Add(tipText);
            elements.Add(restBtn);
            elements.Add(breakBtn);
            elements.Add(countDownText);


            data.Elements = elements;

            return data;
        }
    }
}
