using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Jiffy.Core;
using Jiffy.Utils;
using Jiffy.UI;

namespace Jiffy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Start listening for incoming drops
            DropSyncService.Start();

            // Auto-refresh UI when new drop is received
            DropManager.OnDropReceived = RefreshNearbyDrops;

             
            CurrentSSID.Text = WiFiUtils.GetCurrentSSID();
        }

        // 📂 Drop a file manually
        private void OnDropFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    var drop = DropManager.CreateDrop(filePath);
                    DropSyncService.Send(drop);

                    MessageBox.Show($"📂 Dropped and shared:\n{drop.FileName}", "Drop Successful");
                    RefreshNearbyDrops();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Error dropping file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Allow dragging the window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        // Exit app
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // 🌐 Manual refresh
        private void OnShowAccessible(object sender, RoutedEventArgs e)
        {
            RefreshNearbyDrops();
        }

        // 🔁 Update nearby drop list
        private void RefreshNearbyDrops()
        {
            Dispatcher.Invoke(() =>
            {
                NearbyDropList.Items.Clear();

                var drops = DropManager.GetDropsForCurrentSSID();
                if (!drops.Any())
                {
                    NearbyDropList.Items.Add("No drops on this Wi-Fi.");
                    return;
                }

                foreach (var drop in drops)
                {
                    NearbyDropList.Items.Add(drop);
                }
            });
        }

        // 👀 On double-click: preview or sandbox, then delete
        private void OnDropSelected(object sender, MouseButtonEventArgs e)
        {
            if (NearbyDropList.SelectedItem is FileDrop selectedDrop)
            {
                try
                {
                    // ✅ Preview safe files using PreviewWindow
                    if (PreviewUtils.IsSafeToPreview(selectedDrop.FilePath))
                    {
                        var previewWindow = new PreviewWindow(selectedDrop.FilePath);
                        previewWindow.Show(); // Wait for close before deleting
                    }
                    else
                    {
                        MessageBox.Show("⚠️ Unsafe or large file. Opening in sandbox...");
                        System.Diagnostics.Process.Start("explorer.exe", selectedDrop.FilePath);
                    }

                    // ✅ Delete temporary drop file after viewing
                    if (selectedDrop.IsTemporary && File.Exists(selectedDrop.FilePath))
                    {
                        try
                        {
                            File.Delete(selectedDrop.FilePath);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show($"Could not delete file: {ex.Message}");
                        }
                    }

                    // ✅ Remove from memory/UI
                    DropManager.RemoveDrop(selectedDrop);
                    RefreshNearbyDrops();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
}
