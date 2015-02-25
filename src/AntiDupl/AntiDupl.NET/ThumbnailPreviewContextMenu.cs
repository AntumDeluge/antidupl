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
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace AntiDupl.NET
{
    public class ThumbnailPreviewContextMenu : ContextMenuStrip
    {
        private CoreLib m_core;
        private Options m_options;
        private ThumbnailPreview m_thumbnailPreview;
        private ThumbnailGroupTable m_thumbnailGroupTable;

        private ToolStripMenuItem m_copyPathItem;
        private ToolStripMenuItem m_openImageItem;
        private ToolStripMenuItem m_openFolderItem;
        private ToolStripMenuItem m_renameImageItem;
        private ToolStripMenuItem m_deleteImageItem;
        private ToolStripMenuItem m_selectCurrent;
        private ToolStripMenuItem m_selectAllButThisItemInGroup;
        private ToolStripMenuItem m_unselectCurrent;
        private ToolStripMenuItem m_selectAllInGroup;
        private ToolStripMenuItem m_unselectAllInGroup;

        public ThumbnailPreviewContextMenu(CoreLib core, Options options, ThumbnailPreview thumbnailPreview, ThumbnailGroupTable thumbnailGroupTable)
        {
            m_core = core;
            m_options = options;
            m_thumbnailPreview = thumbnailPreview;
            m_thumbnailGroupTable = thumbnailGroupTable;
            InitializeComponents();
            UpdateStrings();
            Resources.Strings.OnCurrentChange += new Resources.Strings.CurrentChangeHandler(UpdateStrings);
            Opening += new CancelEventHandler(OnOpening);
        }

        private void InitializeComponents()
        {
            RenderMode = ToolStripRenderMode.System;

            m_copyPathItem = InitFactory.MenuItem.Create(null, null, CopyPath);
            m_openImageItem = InitFactory.MenuItem.Create(null, null, OpenImage);
            m_openFolderItem = InitFactory.MenuItem.Create(null, null, OpenFolder);
            m_renameImageItem = InitFactory.MenuItem.Create(null, null, RenameImage);
            m_deleteImageItem = InitFactory.MenuItem.Create(null, null, DeleteImage);
            //m_selectAllButThisItem = InitFactory.MenuItem.Create(null, null, SelectAllButThis);
            //m_selectAllButThisItem = InitFactory.MenuItem.Create(null, null, new EventHandler(MakeSelect(CoreDll.SelectionType.SelectAllButThis)));
            m_selectCurrent = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectCurrent, MakeSelect);
            m_selectAllButThisItemInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectAllButThis, MakeSelect);
            m_unselectCurrent = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.UnselectCurrent, MakeSelect);
            m_selectAllInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectAll, MakeSelect);
            m_unselectAllInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.UnselectAll, MakeSelect);
            
            Items.Add(new ToolStripSeparator());
        }
        
        private void OnOpening(object sender, EventArgs e)
        {
            Items.Clear();
            
            Items.Add(m_copyPathItem);
            Items.Add(new ToolStripSeparator());
            Items.Add(m_openImageItem);
            Items.Add(m_openFolderItem);
            Items.Add(new ToolStripSeparator());
            Items.Add(m_renameImageItem);
            Items.Add(m_deleteImageItem);
            Items.Add(new ToolStripSeparator());
            Items.Add(m_selectCurrent);
            Items.Add(m_selectAllButThisItemInGroup);
            Items.Add(m_unselectCurrent);
            Items.Add(m_selectAllInGroup);
            Items.Add(m_unselectAllInGroup);
        }

        private void UpdateStrings()
        {
            Strings s = Resources.Strings.Current;

            m_copyPathItem.Text = s.ImagePreviewContextMenu_CopyPathItem_Text;
            m_openImageItem.Text = s.ImagePreviewContextMenu_OpenImageItem_Text;
            m_openFolderItem.Text = s.ImagePreviewContextMenu_OpenFolderItem_Text;
            m_renameImageItem.Text = s.ImagePreviewContextMenu_RenameImageItem_Text;
            m_deleteImageItem.Text = "Удалить картинку";
            m_selectCurrent.Text = "Выбрать текущую в этой группе";
            m_selectAllButThisItemInGroup.Text = "Выбрать все кроме этой в этой группе";
            m_unselectCurrent.Text = "Снять выделение с текущей в этой группе";
            m_selectAllInGroup.Text = "Выбрать все в этой группе";
            m_unselectAllInGroup.Text = "Снять выделение со всех в этой группе";
        }

        private void OpenImage(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = m_thumbnailPreview.ImageInfo.path;
            try
            {
                Process.Start(startInfo);
            }
            catch (System.Exception exeption)
            {
                MessageBox.Show(exeption.Message);
            }
        }

        private void OpenFolder(object sender, EventArgs e)
        {
            FolderOpener.OpenContainingFolder(m_thumbnailPreview.ImageInfo);
        }

        private void CopyPath(object sender, EventArgs e)
        {
            Clipboard.SetText(m_thumbnailPreview.ImageInfo.path);
        }

        /// <summary>
        /// Вызов диалогового окна о переименование и переименование.
        /// </summary>
        private void RenameImage(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = m_thumbnailPreview.ImageInfo.path;
            dialog.OverwritePrompt = false;
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = (new FileInfo(m_thumbnailPreview.ImageInfo.path)).Extension;
            dialog.FileOk += new System.ComponentModel.CancelEventHandler(OnRenameImageDialogFileOk);
            dialog.Title = Resources.Strings.Current.ImagePreviewContextMenu_RenameImageItem_Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (m_thumbnailGroupTable.Rename(m_thumbnailPreview.Group, m_thumbnailPreview.Index, dialog.FileName))
                {
                    //m_thumbnailPreview.SetThumbnail(m_thumbnailPreview.Group, m_thumbnailPreview.Index);
                    m_thumbnailGroupTable.UpdateCurrentImagePreview();
                }
            }
        }

        /// <summary>
        /// Проверка правильного имени.
        /// </summary>
        private void OnRenameImageDialogFileOk(object sender, CancelEventArgs e)
        {
            SaveFileDialog dialog = (SaveFileDialog)sender;
            FileInfo oldFileInfo = new FileInfo(m_thumbnailPreview.ImageInfo.path);
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
            }
        }

        private void DeleteImage(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Удалить?",
            //        "Dialog", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
                if (m_thumbnailGroupTable.Delete(m_thumbnailPreview.Group, m_thumbnailPreview.Index))
                {
                    m_thumbnailPreview.SetThumbnail(m_thumbnailPreview.Group, m_thumbnailPreview.Index);
                    m_thumbnailGroupTable.ChangeCurrentThumbnail(m_thumbnailPreview.Group, m_thumbnailPreview.Index);
                    m_thumbnailGroupTable.SelectedResultsChanged();
                }
            //}
        }

        /*private void SelectAllButThis(object sender, EventArgs e)
        {
            m_core.SetSelection(m_thumbnailPreview.Group.id, m_thumbnailPreview.Index, CoreDll.SelectionType.SelectAllButThis);
            //m_thumbnailGroupPanel.Table.ChangeCurrentThumbnail(m_group, m_index);
            //m_thumbnailGroupTable.SelectedResultsChanged();
            m_thumbnailGroupTable.UpdateGroup(m_thumbnailPreview.Group.id);
        }*/

        private void MakeSelect(object sender, EventArgs e)
        {
            if (m_thumbnailGroupTable.GroupNotNull(m_thumbnailPreview.Group.id))
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                CoreDll.SelectionType selection = (CoreDll.SelectionType)item.Tag;
                m_core.SetSelection(m_thumbnailPreview.Group.id, m_thumbnailPreview.Index, selection);
                m_thumbnailGroupTable.UpdateGroup(m_thumbnailPreview.Group.id);
                //m_thumbnailGroupTable.ChangeCurrentThumbnail(m_thumbnailPreview.Group, m_thumbnailPreview.Index);
                m_thumbnailGroupTable.SelectedResultsChanged();
            }
        }
    }
}
