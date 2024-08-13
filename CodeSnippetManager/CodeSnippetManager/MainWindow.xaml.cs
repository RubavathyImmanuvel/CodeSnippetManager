using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace CodeSnippetManager
{
    public class CodeSnippet
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
    }

    public partial class MainWindow : Window
    {
        private List<CodeSnippet> _snippets = new List<CodeSnippet>();
        private string _filePath = "snippets.xml";

        public MainWindow()
        {
            InitializeComponent();
            LoadSnippets();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetsListBox.SelectedItem is CodeSnippet selectedSnippet)
            {
                _snippets.Remove(selectedSnippet);
                SaveSnippets();
                UpdateSnippetsListBox();
                ClearInputs();
            }
        }
        private void LoadSnippets()
        {
            if (File.Exists(_filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CodeSnippet>));
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    _snippets = (List<CodeSnippet>)serializer.Deserialize(reader);
                }
            }

            UpdateSnippetsListBox();
        }

        private void SaveSnippets()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CodeSnippet>));
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                serializer.Serialize(writer, _snippets);
            }
        }

        private void UpdateSnippetsListBox()
        {
            SnippetsListBox.ItemsSource = null;
            SnippetsListBox.ItemsSource = _snippets;
        }

        private void ClearInputs()
        {
            TitleTextBox.Clear();
            ContentTextBox.Clear();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var snippet = new CodeSnippet
            {
                Title = TitleTextBox.Text,
                Content = ContentTextBox.Text,
            };

            _snippets.Add(snippet);
            SaveSnippets();
            UpdateSnippetsListBox();
            ClearInputs();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetsListBox.SelectedItem is CodeSnippet selectedSnippet)
            {
                selectedSnippet.Title = TitleTextBox.Text;
                selectedSnippet.Content = ContentTextBox.Text;

                SaveSnippets();
                UpdateSnippetsListBox();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a snippet to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SnippetsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SnippetsListBox.SelectedItem is CodeSnippet selectedSnippet)
            {
                TitleTextBox.Text = selectedSnippet.Title;
                ContentTextBox.Text = selectedSnippet.Content;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                SnippetsListBox.ItemsSource = _snippets;
            }
            else
            {
                var filteredSnippets = _snippets.Where(snippet =>
                    snippet.Title.ToLower().Contains(searchText) ||
                    snippet.Content.ToLower().Contains(searchText) ||
                    snippet.Category.ToLower().Contains(searchText)).ToList();

                SnippetsListBox.ItemsSource = filteredSnippets;
            }
        }

    }

}
