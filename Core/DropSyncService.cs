using Jiffy.Core;
using System.Windows;

namespace Jiffy.Core
{
    public static class DropSyncService
    {
        // Starts listening for incoming drops from other users on the same network
        public static void Start()
        {
            DropListener.Start();
        }

        // Broadcasts the given FileDrop to other users on the same network
        public static void Send(FileDrop drop)
        {
            try
            {
                DropSender.Broadcast(drop);
            }
            catch (System.Exception ex)
            {
                // Show an error message to avoid crashing your app
                MessageBox.Show($"Failed to send drop:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
