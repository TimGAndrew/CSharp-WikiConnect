using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiConnect.Commands;
using WikiConnect.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikiConnect.DataContext
{
    public class MainDataContext : INotifyPropertyChanged
    {
        private readonly string baseURL = "https://en.wikipedia.org/wiki/";
        private readonly string stepsString = @"{0} → {2} : {1} Degrees";
        private SixDegreesModel model;
        private ObservableCollection<string> steps;
        private string start;
        private string end;
        private ResultLoader loader;
        private bool isLoading;
        private string filter;
        private string selectedStep;
        private Visibility panelVisibility = Visibility.Collapsed;

        public SubmitCommand SubmitCommand { get; set; }

        public Frame Frame { get; set; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                SubmitCommand.FireCanExecuteChanged();
            }
        }

        public string StartArticle {
            get { return start; }
            set
            {
                start = value;
                SubmitCommand.FireCanExecuteChanged();
            }
        }

        public string EndArticle {
            get { return end; }
            set
            {
                end = value;
                SubmitCommand.FireCanExecuteChanged();
            }
        }

        public string Filter {
            get { return filter; }
            set
            {
                filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Steps)));
            }
        }

        public string HTMLText { get; set; }

        public SixDegreesModel Model
        {
            get { return model; }
            set
            {
                if (value != null)
                {
                    model = value;

                    if (model.steps != null)
                    {
                        Steps = new ObservableCollection<string>(model.steps);
                    }
                    
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Steps)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StepsDisplay)));
                }
            }
        }

        public string SelectedStep {
            get { return selectedStep; }
            set
            {
                if (value != null) {
                    selectedStep = value;
                    HTMLText = BuildArticleURL(selectedStep);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HTMLText)));
                }
            }
        }

        public string StepsDisplay {
            get
            {
               return (model != null) ? string.Format(stepsString, model.start_article, model.degrees, model.end_article) : "";
            }
        } 

        public ObservableCollection<string> Steps
        {
            get
            {
                if (!string.IsNullOrEmpty(Filter))
                {
                    string f = Filter.ToLowerInvariant().Trim();
                    return new ObservableCollection<string>(steps.Where(d => d.ToLowerInvariant().
                    Contains(f)).ToList());
                }
                else
                {
                    return steps;
                }

            }
            set
            {
                steps = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Steps)));
            }
        }

        public Visibility PanelVisibility
        {
            get { return panelVisibility; }
            set
            {
                panelVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PanelVisibility)));
            }
        }

        public MainDataContext()
        {
            Steps = new ObservableCollection<string>();
            SubmitCommand = new SubmitCommand(this);
            loader = new ResultLoader();
            loader.OnSearchResultsFetched += new OnResultsFetched(OnResultsFetched);
            loader.OnSearchStarted += new OnSearchStarted(OnSearchStarted);
            loader.OnSearchError += new OnSearchError(OnSearchError);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PerformSearch()
        {
            loader?.GetResultAsync(StartArticle, EndArticle);
        }

        public async void OnResultsFetched(object sender, OnResultsFetchedArgs e)
        {
            if (e.Model.degrees > 0)
            {
                IsLoading = false;
                PanelVisibility = Visibility.Collapsed;
                Model = e.Model;
                Frame?.Navigate(typeof(MainResultsPage));
                SelectedStep = Steps[Steps.Count - 1];
            }
            else
            {
                IsLoading = false;
                PanelVisibility = Visibility.Collapsed;
                ContentDialog aboutAppDialog = new ContentDialog()
                {
                    Title = "No Results",
                    Content = "Your search returned no results",
                    PrimaryButtonText = "Ok"
                };
                
                ContentDialogResult result = await aboutAppDialog.ShowAsync();
            }
        }

        public void OnSearchStarted(object sender, EventArgs e)
        {
            PanelVisibility = Visibility.Visible;
            IsLoading = true;
        }

        public async void OnSearchError(object sender, OnSearchErrorArgs e)
        {
            IsLoading = false;
            PanelVisibility = Visibility.Collapsed;
            ContentDialog aboutAppDialog = new ContentDialog()
            {
                Title = "An Error Occurred",
                Content = "An error occurred while attempting to fetch the results\n" + e.Message,
                PrimaryButtonText = "Ok"
            };

            ContentDialogResult result = await aboutAppDialog.ShowAsync();
        }
        
        public string BuildArticleURL(string article) {
            return baseURL + article.Replace(" ", "_");
        }
    }
}
