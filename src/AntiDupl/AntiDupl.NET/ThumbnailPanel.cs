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
using System.IO;

namespace AntiDupl.NET
{
    /// <summary>
    /// Панель одного изображения и информации о нем, checkbox.
    /// </summary>
    public class ThumbnailPanel :  RaisedPanel
    {
        private const int IBW = 1;//Internal border width
        private const int EBW = 2;//External border width

        private CoreLib m_core;
        public CoreGroup Group { get { return m_group; } }
        private CoreGroup m_group;
        /// <summary>
        /// Индекс картинки в группе.
        /// </summary>
        public int Index { get { return m_index; } }
        private int m_index;
        public CoreImageInfo ImageInfo { get { return m_group.images[m_index]; } }
        private AntiDupl.NET.Options m_options;
        private ThumbnailGroupPanel m_thumbnailGroupPanel;

        private TableLayoutPanel m_mainLayout;
        private TableLayoutPanel m_controlLayout;
        private CheckBox m_checkBox;
        private PictureBox m_pictureBox;
        private TableLayoutPanel m_infoLayout;
        private TableLayoutPanel m_infoLayout2;
        private Label m_fileSizeLabel;
        private Label m_imageSizeLabel;
        private Label m_imageTypeLabel;

        private Label m_imageBlocknessLabel;
        private Label m_imageBlurringLabel;
        private Label m_imageExifLabel;

        private Label m_fileNameLabel;

        /// <summary>
        /// Всплывающая подсказка
        /// </summary>
        private ToolTip m_toolTip;

        public bool Selected
        { 
            get
            {
                return m_checkBox.Checked;
            }
            set
            {
                m_checkBox.Checked = value;
            }
        }
        
        public Bitmap Thumbnail
        {
            get
            {
                return (Bitmap)m_pictureBox.Image;
            }
            set
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        m_pictureBox.Image = value;
                    }));
                }
                else
                {
                    m_pictureBox.Image = value;
                }
            }
        }

        public ThumbnailPanel(CoreLib core, AntiDupl.NET.Options options, CoreGroup group, int index, ThumbnailGroupPanel thumbnailGroupPanel)
        {
            m_core = core;
            m_options = options;
            m_group = group;
            m_index = index;
            m_thumbnailGroupPanel = thumbnailGroupPanel;
            InitializeComponents();

            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;

            SetImageInfo();
        }

        private void InitializeComponents()
        {
            Strings s = Resources.Strings.Current;

            DoubleBuffered = true;
            BackColor = Color.Transparent;

            m_mainLayout = InitFactory.Layout.Create(1, 4, 0, 0);
            m_mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            m_mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            m_mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            m_mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));


            m_controlLayout = InitFactory.Layout.Create(1, 1, 0, 0);
            m_controlLayout.Height = 20;
            m_checkBox = new CheckBox();
            m_checkBox.Location = new Point(0, 0);
            m_checkBox.Margin = new Padding(0);
            m_checkBox.Padding = new Padding(0);
            m_checkBox.Height = 18;
            //m_checkBox.Height = m_checkBox.Font.Height;
            m_checkBox.Click += new EventHandler(OnCheckBoxClick);

            m_controlLayout.Controls.Add(m_checkBox, 0, 0);


            m_pictureBox = new PictureBox();
            m_pictureBox.Location = new Point(0, 0);
            m_pictureBox.ClientSize = m_options.resultsOptions.thumbnailSizeMax;
            m_pictureBox.SizeMode = PictureBoxSizeMode.Zoom; 
            m_pictureBox.BorderStyle = BorderStyle.Fixed3D;
            m_pictureBox.Image = null;
            m_pictureBox.Padding = new Padding(0);
            m_pictureBox.Margin = new Padding(0);
            m_pictureBox.Location = new Point(Padding.Left, Padding.Top);
            m_pictureBox.BackColor = Color.Transparent;
            m_pictureBox.Click += new EventHandler(OnClick);

            m_fileSizeLabel = CreateLabel();
            m_imageSizeLabel = CreateLabel();
            m_imageTypeLabel = CreateLabel();
            m_fileNameLabel = CreateLabel();
            m_imageBlocknessLabel = CreateLabel();
            m_imageBlurringLabel = CreateLabel();

            m_imageExifLabel = new Label();
            m_imageExifLabel.Location = new Point(0, 0);
            m_imageExifLabel.BorderStyle = BorderStyle.Fixed3D;
            m_imageExifLabel.Padding = new Padding(0, 0, 0, 0);
            m_imageExifLabel.Margin = new Padding(0, 0, 0, 0);
            m_imageExifLabel.TextAlign = ContentAlignment.TopCenter;
            m_imageExifLabel.AutoSize = false;
            m_imageExifLabel.FlatStyle = FlatStyle.System;
            m_imageExifLabel.Text = s.ImagePreviewPanel_EXIF_Text;
            //m_imageExifLabel.Visible = false;

            m_infoLayout = InitFactory.Layout.Create(3, 1, 0, 0);
            m_infoLayout.Height = 16;
            //m_infoLayout.Height = (m_imageSizeLabel.Font.Height + m_imageSizeLabel.Margin.Vertical + m_imageSizeLabel.Padding.Vertical + 3) * 2;*/

            m_infoLayout.Controls.Add(m_fileSizeLabel, 0, 0);
            m_infoLayout.Controls.Add(m_imageSizeLabel, 1, 0);
            m_infoLayout.Controls.Add(m_imageTypeLabel, 2, 0);

            m_infoLayout2 = InitFactory.Layout.Create(3, 1, 0, 0);
            m_infoLayout2.Height = 16;

            m_infoLayout2.Controls.Add(m_imageBlocknessLabel, 0, 1);
            m_infoLayout2.Controls.Add(m_imageBlurringLabel, 1, 1);
            m_infoLayout2.Controls.Add(m_imageExifLabel, 2, 1);

            m_mainLayout.Controls.Add(m_controlLayout, 0, 0);
            m_mainLayout.Controls.Add(m_pictureBox, 0, 1);
            m_mainLayout.Controls.Add(m_infoLayout, 0, 2);
            m_mainLayout.Controls.Add(m_infoLayout2, 0, 3);
            //m_mainLayout.Controls.Add(m_fileNameLabel, 0, 3);

            m_toolTip = new ToolTip();
            m_toolTip.ShowAlways = true;
            m_toolTip.SetToolTip(m_imageBlocknessLabel, s.ResultsListView_Blockiness_Column_Text);
            m_toolTip.SetToolTip(m_imageBlurringLabel, s.ResultsListView_Blurring_Column_Text);

            Controls.Add(m_mainLayout);

            SetSize();
        }

        private Label CreateLabel()
        {
            Label label = new Label();
            label.Location = new Point(0, 0);
            label.Padding = new Padding(0, 0, 0, 0);
            label.Margin = new Padding(0, 0, 0, 0);
            label.TextAlign = ContentAlignment.TopCenter;
            label.AutoSize = false;
            label.BorderStyle = BorderStyle.Fixed3D;
            label.Height = label.Font.Height + 2;
            label.FlatStyle = FlatStyle.System;
            label.Text = "0";
            label.Click += new EventHandler(OnClick);
            return label;
        }

        private void SetSize()
        {
            m_checkBox.Width = 128;

            m_fileSizeLabel.Width = 40;
            m_imageSizeLabel.Width = 60;
            m_imageTypeLabel.Width = 28;

            m_imageBlocknessLabel.Width = 40;
            m_imageBlurringLabel.Width = 40;
            m_imageExifLabel.Width = 48;
            
            m_fileNameLabel.Width = 128;
            m_fileNameLabel.Location = new Point(0, 0);

            Font font = m_fileSizeLabel.Font;
            int width = m_pictureBox.Width + Padding.Horizontal;
            /*int height = m_pictureBox.Height + 
                (font.Height + m_fileSizeLabel.Margin.Vertical + m_fileSizeLabel.Padding.Vertical +
                m_infoLayout.Padding.Vertical)*4 + 
                m_mainLayout.Padding.Vertical + m_mainLayout.Margin.Vertical + Padding.Vertical + 8;*/
            int height = m_controlLayout.Height +
                         m_pictureBox.Height +
                         m_infoLayout.Height +
                         m_infoLayout2.Height + 
                         //font.Height + m_fileSizeLabel.Margin.Vertical + m_fileSizeLabel.Padding.Vertical + m_infoLayout.Padding.Vertical +
                         m_mainLayout.Padding.Vertical + m_mainLayout.Margin.Vertical + Padding.Vertical + 2;
            ClientSize = new Size(width, height);
        }

        private void SetImageInfo()
        {
            CoreImageInfo info = m_group.images[m_index];

            m_fileSizeLabel.Text = info.GetFileSizeString();
            m_imageSizeLabel.Text = string.Format("{0}×{1}", info.width, info.height);
            m_imageTypeLabel.Text = (info.type == CoreDll.ImageType.None ? "   " : info.GetImageTypeString());
            m_checkBox.Text = m_fileNameLabel.Text = Path.GetFileNameWithoutExtension(info.path);
            m_toolTip.SetToolTip(m_checkBox, info.path);
            //m_toolTip.SetToolTip(m_fileNameLabel, info.path);
            m_imageBlocknessLabel.Text = info.blockiness.ToString("F3");
            m_imageBlurringLabel.Text = info.blurring.ToString("F3");
            if (info.exifInfo.isEmpty == CoreDll.FALSE)
            {
                m_imageExifLabel.Visible = true;
                SetExifTooltip(info);
                m_imageExifLabel.ForeColor = m_group.exifDiffrent ? Color.Red : TableLayoutPanel.DefaultForeColor;
            }
            else
                m_imageExifLabel.Visible = false;

            //устанавливаем подсветку
            m_fileSizeLabel.ForeColor = info.size == m_group.fileSizeMax ? TableLayoutPanel.DefaultForeColor : Color.Red;
            m_imageSizeLabel.ForeColor = (uint)(info.width * info.height) == m_group.imageSizeMax ? TableLayoutPanel.DefaultForeColor : Color.Red;
            m_imageTypeLabel.ForeColor = m_group.imageTypeDiffrent ? Color.Red : TableLayoutPanel.DefaultForeColor;
            m_imageBlocknessLabel.ForeColor = info.blockiness == m_group.imageBlocknessMax ? TableLayoutPanel.DefaultForeColor : Color.Red;
            m_imageBlurringLabel.ForeColor = info.blurring == m_group.imageBlurringMax ? TableLayoutPanel.DefaultForeColor : Color.Red;

            bool[] selected = m_core.GetSelection(m_group.id, (uint)m_index, 1);
            m_checkBox.Checked = selected[0];
        }

        /// <summary>
        /// Устанавливаем выделенным текущий.
        /// </summary>
        private void OnCheckBoxClick(object sender, EventArgs e)
        {
            if (m_checkBox.Checked)
            {
                m_core.SetSelection(m_group.id, m_index, CoreDll.SelectionType.SelectCurrent);
            }
            else
            {
                m_core.SetSelection(m_group.id, m_index, CoreDll.SelectionType.UnselectCurrent);
            }
            m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, m_index);
            m_thumbnailGroupPanel.Table.SelectedResultsChanged();
        }

        /// <summary>
        /// Обновляем превью.
        /// </summary>
        //bool clicked;
        private void OnClick(object sender, EventArgs e)
        {
            //clicked = true;
            //Invalidate();
            m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, m_index);
            //this.Selected = true;
            //this.Focus = true;
            //this.ForeColor = Color.Red;
            //if (this.Focused)
            //this.BackColor = Color.Olive;
            //
            this.Focus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
        }
        protected override bool IsInputKey(Keys keyData)
        {
            /*if (keyData == Keys.Up || keyData == Keys.Down) return true;
            if (keyData == Keys.Left || keyData == Keys.Right) return true;*/
            m_thumbnailGroupPanel.Table.OnChangeKey(keyData, m_group.id, m_index);
            /*switch (keyData)
            {
                case Keys.Left:
                    m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, --m_index);
                    break;
                case Keys.Right:
                    m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, ++m_index);
                    break;
            }*/
            return base.IsInputKey(keyData);
        }
        protected override void OnEnter(EventArgs e)
        {
            if (this.Focused)
                m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, m_index);
            this.Invalidate();
            base.OnEnter(e);
        }
        protected override void OnLeave(EventArgs e)
        {
            //this.BackColor = Color.Transparent;
            this.Invalidate();
            base.OnLeave(e);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (this.Focused)
            {
                var rc = this.ClientRectangle;
                //rc.Inflate(+2, +2);
                //rc.Inflate(-2, -2);
                ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
                //this.BackColor = Color.Red;
            }
        }

        /*protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //if (this.Selected)
            //    this.BackColor = Color.Red;
            /*
            if (clicked)
            {
                //var g = e.Graphics;
                //var b = new SolidBrush(Color.FromArgb(50, 50, 50, 50));
                //g.FillRectangle(b, mouse.X, mouse.Y, 50, 50);
                this.BackColor = Color.Red;

                clicked = false;
            }
            else
                this.BackColor = Color.Transparent;
        }*/

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
        private void SetExifTooltip(CoreImageInfo imageInfo)
        {
            Strings s = Resources.Strings.Current;
            string exifSting = String.Empty;

            List<string> exifList = GetExifList(imageInfo, s);

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
