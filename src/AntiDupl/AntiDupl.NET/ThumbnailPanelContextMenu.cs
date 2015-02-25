using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AntiDupl.NET
{
    public class ThumbnailPanelContextMenu : ContextMenuStrip
    {
        private CoreLib m_core;
        private Options m_options;
        private ThumbnailPanel m_thumbnailPanel;
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
        private ToolStripMenuItem m_unselectAll;

        public ThumbnailPanelContextMenu(CoreLib core, Options options, ThumbnailPanel thumbnailPanel, ThumbnailGroupTable thumbnailGroupTable)
        {
            m_core = core;
            m_options = options;
            m_thumbnailPanel = thumbnailPanel;
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

            m_selectCurrent = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectCurrent, MakeSelect);
            m_selectAllButThisItemInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectAllButThis, MakeSelect);
            m_unselectCurrent = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.UnselectCurrent, MakeSelect);
            m_selectAllInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.SelectAll, MakeSelect);
            m_unselectAllInGroup = InitFactory.MenuItem.Create(null, CoreDll.SelectionType.UnselectAll, MakeSelect);
            m_unselectAll = InitFactory.MenuItem.Create(null, null, UnselectAll);
            
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
            Items.Add(m_unselectAll);
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
            m_unselectAll.Text = "Снять выделение со всех";
        }

        private void OpenImage(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = m_thumbnailPanel.ImageInfo.path;
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
            FolderOpener.OpenContainingFolder(m_thumbnailPanel.ImageInfo);
        }

        private void CopyPath(object sender, EventArgs e)
        {
            //Clipboard.SetText(m_thumbnailPreview.ImageInfo.path);
        }

        /// <summary>
        /// Вызов диалогового окна о переименование и переименование.
        /// </summary>
        private void RenameImage(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = m_thumbnailPanel.ImageInfo.path;
            dialog.OverwritePrompt = false;
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = (new FileInfo(m_thumbnailPanel.ImageInfo.path)).Extension;
            dialog.FileOk += new System.ComponentModel.CancelEventHandler(OnRenameImageDialogFileOk);
            dialog.Title = Resources.Strings.Current.ImagePreviewContextMenu_RenameImageItem_Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (m_thumbnailGroupTable.Rename(m_thumbnailPanel.Group, m_thumbnailPanel.Index, dialog.FileName))
                //if (m_thumbnailGroupTable.Rename(m_thumbnailPanel.Group, m_thumbnailPanel.Index, dialog.FileName))
                {
                    m_thumbnailGroupTable.MainSplitContainer.ThumbnailPreview.SetThumbnail(m_thumbnailPanel.Group, m_thumbnailPanel.Index);
                }
            }
        }

        /// <summary>
        /// Проверка правильного имени.
        /// </summary>
        private void OnRenameImageDialogFileOk(object sender, CancelEventArgs e)
        {
            SaveFileDialog dialog = (SaveFileDialog)sender;
            FileInfo oldFileInfo = new FileInfo(m_thumbnailPanel.ImageInfo.path);
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
            /*if (m_thumbnailGroupTable.Delete(m_thumbnailPanel.Group, m_thumbnailPanel.Index))
            {
                m_thumbnailPreview.SetThumbnail(m_thumbnailPreview.Group, m_thumbnailPreview.Index);
            }*/
            m_thumbnailGroupTable.Delete(m_thumbnailPanel.Group, m_thumbnailPanel.Index);
            m_thumbnailGroupTable.ChangeCurrentThumbnail(m_thumbnailPanel.Group, m_thumbnailPanel.Index);
            m_thumbnailGroupTable.SelectedResultsChanged();
        }

        private void MakeSelect(object sender, EventArgs e)
        {
            if (m_thumbnailGroupTable.GroupNotNull(m_thumbnailPanel.Group.id))
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                CoreDll.SelectionType selection = (CoreDll.SelectionType)item.Tag;
                m_core.SetSelection(m_thumbnailPanel.Group.id, m_thumbnailPanel.Index, selection);
                m_thumbnailGroupTable.UpdateGroup(m_thumbnailPanel.Group.id);
                m_thumbnailGroupTable.SelectedResultsChanged();
            }
        }

        private void UnselectAll(object sender, EventArgs e)
        {
            m_thumbnailGroupTable.UnselectAll();
            m_thumbnailGroupTable.SelectedResultsChanged();
        }
    }
}
