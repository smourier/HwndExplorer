using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HwndExplorer.Utilities
{
    public static class WinformsUtilities
    {
        public const int WHERE_NOONE_CAN_SEE_ME = -32000; // from \windows\core\ntuser\kernel\userk.h

        public static string ApplicationName => Resources.AppName;
        public static string? ApplicationVersion => AssemblyUtilities.GetFileVersion(Assembly.GetExecutingAssembly());
        public static string ApplicationTitle => ApplicationName + " V" + ApplicationVersion;

        public static void ShowMessage(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static DialogResult ShowConfirm(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        public static DialogResult ShowQuestion(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        public static void ShowError(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static void ShowWarning(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void ShowMessageWithTimeout(this IWin32Window owner, string text, int timeout = 30000)
        {
            if (timeout <= 0 || timeout == int.MaxValue)
            {
                ShowMessage(owner, text);
                return;
            }

            owner.MessageBoxTimeout(text, ApplicationTitle, (int)MessageBoxIcon.Information, timeout);
        }

        public static void ShowErrorWithTimeout(this IWin32Window owner, string text, int timeout = 30000)
        {
            if (timeout <= 0 || timeout == int.MaxValue)
            {
                ShowError(owner, text);
                return;
            }

            owner.MessageBoxTimeout(text, ApplicationTitle + " - " + Resources.Error, (int)MessageBoxIcon.Error, timeout);
        }

        public static void ShowWarningWithTimeout(this IWin32Window owner, string text, int timeout = 30000)
        {
            if (timeout <= 0 || timeout == int.MaxValue)
            {
                ShowWarning(owner, text);
                return;
            }

            owner.MessageBoxTimeout(text, ApplicationTitle + " - " + Resources.Warning, (int)MessageBoxIcon.Warning, timeout);
        }

        public static TreeNode? FindByFullPath(this TreeView treeView, string fullPath)
        {
            ArgumentNullException.ThrowIfNull(treeView);
            ArgumentNullException.ThrowIfNull(fullPath);

            if (fullPath.StartsWith(Path.AltDirectorySeparatorChar.ToString()) || fullPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fullPath = fullPath[1..];
            }

            foreach (var child in treeView.Nodes.Cast<TreeNode>())
            {
                if (child.FullPath == fullPath)
                    return child;

                var found = FindByFullPath(child, fullPath);
                if (found != null)
                    return found;
            }
            return null;
        }

        public static TreeNode? FindByFullPath(this TreeNode treeNode, string fullPath)
        {
            ArgumentNullException.ThrowIfNull(treeNode);
            ArgumentNullException.ThrowIfNull(fullPath);

            if (fullPath.StartsWith(Path.AltDirectorySeparatorChar.ToString()) || fullPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fullPath = fullPath[1..];
            }

            foreach (var child in treeNode.Nodes.Cast<TreeNode>())
            {
                if (child.FullPath == fullPath)
                    return child;

                var found = FindByFullPath(child, fullPath);
                if (found != null)
                    return found;
            }
            return null;
        }

        public static IEnumerable<TreeNode> EnumerateNodeAndParents(this TreeNode? node)
        {
            if (node == null)
                yield break;

            yield return node;
            foreach (var parent in EnumerateNodeAndParents(node.Parent))
            {
                yield return parent;
            }
        }

        public static IEnumerable<object?> EnumerateNodeAndParentsTag(this TreeNode? node)
        {
            if (node == null)
                yield break;

            yield return node.Tag;
            foreach (var parent in EnumerateNodeAndParents(node.Parent))
            {
                yield return parent.Tag;
            }
        }

        public static IEnumerable<T?> EnumerateNodeAndParentsTag<T>(this TreeNode? node)
        {
            if (node == null)
                yield break;

            if (node.Tag != null)
            {
                if (typeof(T).IsAssignableFrom(node.Tag.GetType()))
                    yield return (T)node.Tag;
            }

            foreach (var parent in EnumerateNodeAndParents(node.Parent))
            {
                if (parent.Tag != null)
                {
                    if (typeof(T).IsAssignableFrom(parent.Tag.GetType()))
                        yield return (T)parent.Tag;
                }
            }
        }

        public static T? GetFirstNodeWhere<T>(this TreeNode? node, Func<TreeNode, T?> predicate) where T : class
        {
            if (node == null)
                return default;

            var item = predicate(node);
            if (item != default)
                return item;

            foreach (var child in node.Nodes.Cast<TreeNode>())
            {
                item = GetFirstNodeWhere(child, predicate);
                if (item != default)
                    return item;
            }
            return default;
        }

        public static void SetImageIndex(this TreeNode node, ImageLibraryIndex index) => SetImageIndex(node, (int)index);
        public static void SetImageIndex(this TreeNode node, int index)
        {
            if (node == null)
                return;

            node.ImageIndex = index;
            node.SelectedImageIndex = index;
        }

        public static void SafeBeginInvoke(this Control control, Action action)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
                return;
            }

            action();
        }

        public static IAsyncResult BeginInvoke(this Control control, Action action)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return control.BeginInvoke(action);
        }

        public static Task<T?> BeginInvoke<T>(this Control control, Func<T> action)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!control.IsHandleCreated)
                return Task.FromResult<T?>(default);

            return Task.Factory.FromAsync(control.BeginInvoke(action), r => (T?)control.EndInvoke(r));
        }
    }
}
