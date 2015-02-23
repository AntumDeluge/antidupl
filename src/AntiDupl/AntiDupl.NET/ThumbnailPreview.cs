/*
* AntiDupl.NET Program.
*
* Copyright (c) 2002-2015 Yermalayeu Ihar.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy 
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
* copies of the Software, and to permit persons to whom the Software is 
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in 
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace AntiDupl.NET
{
    /// <summary>
    /// Панель превью изображения.
    /// </summary>
    public class ThumbnailPreview : TableLayoutPanel//Panel
    //TODo надо наследовать от ResultsPreviewBase
    {
        private CoreLib m_core;
        private Options m_options;
        private MainSplitContainer m_mainSplitContainer;
        //private ResultsPreviewDefect m_resultsPreviewDefect;
        
        private CoreGroup m_group = null;
        public CoreGroup Group { get { return m_group; } }
        private int m_index = 0;
        public int Index { get { return m_index; } }
        public CoreImageInfo ImageInfo { get { return m_group.images[m_index]; } }

        private const int IBW = 1;//Internal border width
        private const int EBW = 2;//External border width

        /// <summary>
        /// Панель предпросмотра изображения
        /// </summary>
        private PictureBoxPanel m_imagePreviewPanel;
        private Label m_fileSizeLabel;
        private Label m_imageSizeLabel;
        private Label m_imageTypeLabel;
        private Label m_imageBlocknessLabel;
        private Label m_imageBlurringLabel;
        private Label m_imageExifLabel;
        private Label m_pathLabel;
        private ToolTip m_toolTip;
        /*protected ToolStripPanel m_toolStripPanel;
        protected ToolStrip m_toolStrip;
        private ToolStripButton m_deleteButton;
        private ToolStripButton m_mistakeButton;*/

        /// <summary>
        /// Панель в виде сетки слжержит в себе панели с кнопками и изображением.
        /// </summary>
        protected TableLayoutPanel m_mainLayout;
        /// <summary>
        /// Панель с изображением.
        /// </summary>
        protected TableLayoutPanel m_imageLayout;
        /// <summary>
        /// Панель с кнопками.
        /// </summary>
        protected TableLayoutPanel m_buttonLayout;
        
        public ThumbnailPreview(CoreLib core, Options options, MainSplitContainer mainSplitContainer)
        {
            m_core = core;
            m_options = options;
            m_mainSplitContainer = mainSplitContainer;
            InnitializeComponents();
            //InnitializeTestButton();
        }
        
        private void InnitializeComponents()
        {
            Strings s = Resources.Strings.Current;

            Location = new System.Drawing.Point(0, 0);
            Dock = DockStyle.Fill;

            m_imagePreviewPanel = new PictureBoxPanel(m_core, m_options);

            /*m_toolStrip = new ToolStrip();
            m_toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            m_toolStrip.RenderMode = ToolStripRenderMode.System;
            m_toolStrip.Renderer = new CustomToolStripSystemRenderer();
            m_toolStrip.AutoSize = true;

            m_toolStripPanel = new ToolStripPanel();
            m_toolStripPanel.BackColor = SystemColors.Control;

            m_toolStripPanel.Orientation = Orientation.Vertical;


            /* m_imageLayout = InitFactory.Layout.Create(1, 1, 0, 0);
             m_imageLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
             //m_imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
             //m_imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
             m_imageLayout.Controls.Add(m_imagePreviewPanel, 0, 0);
             m_imageLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;*/

            /*m_buttonLayout = InitFactory.Layout.CreateVerticalCompensatedCenterAlign(30, 0);
            m_buttonLayout.Controls.Add(m_toolStripPanel, 0, 2);
            m_buttonLayout.AutoSize = true;

            m_mainLayout = InitFactory.Layout.Create(2, 1, 0, 0);
            m_mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            m_mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            //m_mainLayout.Controls.Add(m_imageLayout, 0, 0);
            m_mainLayout.Controls.Add(m_imagePreviewPanel, 0, 0);
            m_mainLayout.Controls.Add(m_buttonLayout, 1, 0);

            m_deleteButton = InitFactory.ToolButton.Create("DeleteDefectVerticalButton", CoreDll.LocalActionType.DeleteDefect, OnButtonClicked);
            //m_mistakeButton = InitFactory.ToolButton.Create("MistakeButton", CoreDll.LocalActionType.Mistake, OnButtonClicked);

            /*m_buttonLayout.AutoSize = true;
            AddItems();
            m_toolStripPanel.Controls.Add(m_toolStrip);
            m_toolStripPanel.Size = new Size(m_toolStrip.PreferredSize.Width + 1, m_toolStrip.PreferredSize.Height + 1);

            m_toolStripPanel.Controls.Add(m_toolStrip);
            m_toolStripPanel.Size = new Size(m_toolStrip.PreferredSize.Width + 1, m_toolStrip.PreferredSize.Height + 1);
            Controls.Add(m_mainLayout);*/

            m_fileSizeLabel = new Label();
            m_fileSizeLabel.Dock = DockStyle.Fill;
            m_fileSizeLabel.BorderStyle = BorderStyle.Fixed3D;
            m_fileSizeLabel.Padding = new Padding(1, 3, 1, 0);
            m_fileSizeLabel.TextAlign = ContentAlignment.TopCenter;
            m_fileSizeLabel.AutoSize = true;

            m_imageSizeLabel = new Label();
            m_imageSizeLabel.Dock = DockStyle.Fill;
            m_imageSizeLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageSizeLabel.Padding = new Padding(1, 3, 1, 0);
            m_imageSizeLabel.Margin = new Padding(IBW, 0, 0, 0);
            m_imageSizeLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageSizeLabel.AutoSize = true;

            m_imageBlocknessLabel = new Label();
            m_imageBlocknessLabel.Dock = DockStyle.Fill;
            m_imageBlocknessLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageBlocknessLabel.Padding = new Padding(1, 3, 1, 0);
            m_imageBlocknessLabel.Margin = new Padding(IBW, 0, 0, 0);
            m_imageBlocknessLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageBlocknessLabel.AutoSize = true;

            m_imageBlurringLabel = new Label();
            m_imageBlurringLabel.Dock = DockStyle.Fill;
            m_imageBlurringLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageBlurringLabel.Padding = new Padding(1, 3, 1, 0);
            m_imageBlurringLabel.Margin = new Padding(IBW, 0, 0, 0);
            m_imageBlurringLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageBlurringLabel.AutoSize = true;

            m_imageTypeLabel = new Label();
            m_imageTypeLabel.Dock = DockStyle.Fill;
            m_imageTypeLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageTypeLabel.Padding = new Padding(1, 3, 1, 0);
            m_imageTypeLabel.Margin = new Padding(IBW, 0, 0, 0);
            m_imageTypeLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageTypeLabel.AutoSize = true;

            m_imageExifLabel = new Label();
            m_imageExifLabel.Dock = DockStyle.Fill;
            m_imageExifLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageExifLabel.Padding = new Padding(1, 3, 1, 0);
            m_imageExifLabel.Margin = new Padding(IBW, 0, 0, 0);
            m_imageExifLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageExifLabel.AutoSize = true;
            m_imageExifLabel.Text = s.ImagePreviewPanel_EXIF_Text;
            m_imageExifLabel.Visible = false;

            m_pathLabel = new Label();
            m_pathLabel.Location = new Point(0, 0);
            m_pathLabel.Dock = DockStyle.Fill;
            m_pathLabel.BorderStyle = BorderStyle.Fixed3D;
            m_pathLabel.Padding = new Padding(1, 3, 1, 0);
            m_pathLabel.AutoEllipsis = true;
            m_pathLabel.DoubleClick += new EventHandler(RenameImage);

            m_toolTip = new ToolTip();
            m_toolTip.ShowAlways = true;
            m_toolTip.SetToolTip(m_imageBlocknessLabel, s.ResultsListView_Blockiness_Column_Text);
            m_toolTip.SetToolTip(m_imageBlurringLabel, s.ResultsListView_Blurring_Column_Text);
            // Свойство AutomaticDelay позволяет установить одно значение задержки, которое затем используется для установки значений свойствAutoPopDelay, InitialDelay и ReshowDelay. Каждый раз при установке свойства AutomaticDelay устанавливаются следующие значения по умолчанию.
            //m_toolTip.AutomaticDelay = 500;
            // Интервал времени, в миллисекундах, в течение которого указатель мыши должен оставаться в границах элемента управления, прежде чем появится окно всплывающей подсказки.
            // Равно значению свойства AutomaticDelay. 
            m_toolTip.InitialDelay = 500;
            // Получает или задает интервал времени, который должен пройти перед появлением окна очередной всплывающей подсказки при перемещении указателя мыши с одного элемента управления на другой.
            // Одна пятая значения свойства AutomaticDelay. 
            m_toolTip.ReshowDelay = 1;
            // Период времени, в миллисекундах, ToolTip остается видимыми, когда указатель неподвижн на элементе управления. Значение по умолчанию - 5000. 
            // В десять раз больше, чем значение свойства AutomaticDelay. 
            // you cannot set the AutoPopDelay time higher than an Int16.MaxValue (i.e. 32767) and have it working. Using the tooltip Show() method leads to the same result. Any value higher than 32767 leads the timer to be reset to 5000ms.
            m_toolTip.AutoPopDelay = Int16.MaxValue;


            TableLayoutPanel infoLayout = InitFactory.Layout.Create(7, 1); //number of controls in panel
            infoLayout.Height = m_imageSizeLabel.Height;

            m_pathLabel.TextAlign = ContentAlignment.TopLeft;

            m_fileSizeLabel.Margin = new Padding(EBW, 0, 0, 0);
            m_pathLabel.Margin = new Padding(IBW, 0, EBW, 0);

            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//fileSizeLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//imageSizeLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//imageBlocknessLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//imageBlurringLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//imageTypeLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));//imageExifLabel
            infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));//pathLabel

            infoLayout.Controls.Add(m_fileSizeLabel, 0, 0);
            infoLayout.Controls.Add(m_imageSizeLabel, 1, 0);
            infoLayout.Controls.Add(m_imageBlocknessLabel, 2, 0);
            infoLayout.Controls.Add(m_imageBlurringLabel, 3, 0);
            infoLayout.Controls.Add(m_imageTypeLabel, 4, 0);
            infoLayout.Controls.Add(m_imageExifLabel, 5, 0);
            infoLayout.Controls.Add(m_pathLabel, 6, 0);

           

            //Controls.Clear();
            //RowStyles.Clear();

            m_imagePreviewPanel.Margin = new Padding(EBW, EBW, EBW, IBW);
            infoLayout.Margin = new Padding(0, 0, 0, EBW);

            RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(m_imagePreviewPanel, 0, 0);
            Controls.Add(infoLayout, 0, 1);


            //Controls.Add(m_imagePreviewPanel);
           
        }

       /* private void AddItems()
        {
            m_toolStrip.Items.Add(m_deleteButton);
            m_toolStrip.Items.Add(new ToolStripSeparator());
            m_toolStrip.Items.Add(m_mistakeButton);
        }*/

        //TODO кнопка не появляется!
        private void InnitializeTestButton()
        {
            Button testButton = new Button();
            testButton.Text = "Test";
            testButton.Location = new Point(10, 10);
            //testButton.Location = new Point(10, m_imagePreviewPanel.Location.Y + 10);
            testButton.AutoSize = true;
            //testButton.AutoSize = false;
            //testButton.Width = 500;
            //testButton.Height = 500;
            testButton.Click += (sender, e) =>
            {
                m_mainSplitContainer.UpdateResults();
            };
            testButton.BringToFront();
            Controls.Add(testButton);
        }

        public void SetThumbnail(CoreGroup group, int index)
        {
            m_group = group;
            m_index = index;
            m_imagePreviewPanel.UpdateImage(ImageInfo);
            m_imagePreviewPanel.UpdateImagePadding(m_group.sizeMax);

            m_fileSizeLabel.Text = ImageInfo.GetFileSizeString();
            m_imageSizeLabel.Text = ImageInfo.GetImageSizeString();
            m_imageBlocknessLabel.Text = ImageInfo.GetBlockinessString();
            m_imageBlurringLabel.Text = ImageInfo.GetBlurringString();
            m_imageTypeLabel.Text = ImageInfo.type == CoreDll.ImageType.None ? "   " : ImageInfo.GetImageTypeString();
            if (ImageInfo.exifInfo.isEmpty == CoreDll.FALSE)
            {
                m_imageExifLabel.Visible = true;
                SetExifTooltip(ImageInfo);
            }
            else
                m_imageExifLabel.Visible = false;
            m_pathLabel.Text = ImageInfo.path;
        }

        private void OnButtonClicked(object sender, System.EventArgs e)
        {
            /*ToolStripButton item = (ToolStripButton)sender;
            CoreDll.LocalActionType action = (CoreDll.LocalActionType)item.Tag;
            m_resultsListView.MakeAction(action, CoreDll.TargetType.Current);*/

            //m_core.DeleteSelection(m_group.id, m_index);
        }

        public void RenameImage(object sender, EventArgs e)
        {
            /*FileInfo fileInfo = new FileInfo(m_currentImageInfo.path);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = fileInfo.FullName;
            dialog.OverwritePrompt = false;
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = fileInfo.Extension;
            dialog.FileOk += new System.ComponentModel.CancelEventHandler(OnRenameImageDialogFileOk);
            dialog.Title = Resources.Strings.Current.ImagePreviewContextMenu_RenameImageItem_Text;
            dialog.InitialDirectory = fileInfo.Directory.ToString();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                m_resultsListView.RenameCurrent(m_renameCurrentType, dialog.FileName);
            }*/
        }

        private void OnRenameImageDialogFileOk(object sender, CancelEventArgs e)
        {
            /*SaveFileDialog dialog = (SaveFileDialog)sender;
            FileInfo oldFileInfo = new FileInfo(m_currentImageInfo.path);
            FileInfo newFileInfo = new FileInfo(dialog.FileName);
            if (newFileInfo.FullName != oldFileInfo.FullName && newFileInfo.Exists)
            {
                MessageBox.Show(Resources.Strings.Current.ErrorMessage_FileAlreadyExists,
                    dialog.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else if (newFileInfo.Extension != oldFileInfo.Extension && newFileInfo.Extension.Length > 0)
            {
                e.Cancel = MessageBox.Show(Resources.Strings.Current.WarningMessage_ChangeFileExtension,
                    dialog.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel;
            }*/
        }

        private List<string> GetExifList(CoreImageInfo currentImageInfo, Strings s)
        {
            List<string> exifList = new List<string>();
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.imageDescription))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_ImageDescription + currentImageInfo.exifInfo.imageDescription);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.equipMake))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_EquipMake + currentImageInfo.exifInfo.equipMake);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.equipModel))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_EquipModel + currentImageInfo.exifInfo.equipModel);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.softwareUsed))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_SoftwareUsed + currentImageInfo.exifInfo.softwareUsed);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.dateTime))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_DateTime + currentImageInfo.exifInfo.dateTime);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.artist))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_Artist + currentImageInfo.exifInfo.artist);
            if (!String.IsNullOrEmpty(currentImageInfo.exifInfo.userComment))
                exifList.Add(s.ImagePreviewPanel_EXIF_Tooltip_UserComment + currentImageInfo.exifInfo.userComment);
            return exifList;
        }

        /// <summary>
        /// Устанавливает значение подсказки tooltip для надписи EXIF.
        /// </summary>
        private void SetExifTooltip(CoreImageInfo currentImageInfo)
        {
            Strings s = Resources.Strings.Current;
            string exifSting = String.Empty;

            List<string> exifList = GetExifList(currentImageInfo, s);

            if (exifList.Count > 0)
            {
                for (int i = 0; i < exifList.Count - 1; i++)
                {
                    exifSting = exifSting + exifList[i];
                    exifSting = exifSting + Environment.NewLine;
                }
                exifSting = exifSting + exifList[exifList.Count - 1];

                m_toolTip.SetToolTip(m_imageExifLabel, exifSting);
            }
        }
    }
}
