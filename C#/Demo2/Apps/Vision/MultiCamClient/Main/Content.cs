using MultiCamApp.Connection;
using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace MultiCamApp.Main
{
    public class Content : ObservableProperty
    {
        public UserControl CurrentContent
        {
            get => currentContent;
            set => SetField(value, ref currentContent);
        }


        UserControl currentContent;
        Dictionary<string, UserControl> contents = new Dictionary<string, UserControl>();


        public Content()
        {
            contents.Add(nameof(ConnectionView), new ConnectionView());

            currentContent = contents[nameof(ConnectionView)];
        }


        public void ShowConnection() => CurrentContent = contents[nameof(ConnectionView)];
    }
}