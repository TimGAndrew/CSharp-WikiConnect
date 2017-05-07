using System;
using System.Windows.Input;
using WikiConnect.DataContext;

namespace WikiConnect.Commands
{
    public class SubmitCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public MainDataContext Context { get; }

        public SubmitCommand(MainDataContext context)
        {
            Context = context;
        }

        public bool CanExecute(object parameter)
        {
            return (!string.IsNullOrEmpty(Context.StartArticle)
                && !string.IsNullOrEmpty(Context.EndArticle) && !Context.IsLoading);
        }

        public void Execute(object parameter)
        {
            Context.PerformSearch();
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
