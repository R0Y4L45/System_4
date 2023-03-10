using Car_Generator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace Car_Generator;
public partial class MainWindow : Window
{
    CancellationTokenSource? cts = new();
    Stopwatch? stopwatch = null;
    public MainWindow()
    {
        InitializeComponent();
    }
    public void SingleMode(CancellationToken cancellationTokenSource)
    {
        var directory = new DirectoryInfo(@"..\..\..\Car Data");

        new Thread(() =>
        {
            foreach (var item in directory.GetFiles())
            {
                stopwatch = Stopwatch.StartNew();

                if (item.Extension == ".json")
                {
                    var jsonText = File.ReadAllText(item.FullName);
                    var listCar = JsonSerializer.Deserialize<List<Car>>(jsonText);

                    if (listCar is null)
                        continue;


                    foreach (var car in listCar)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            stopwatch.Stop();
                            Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                            break;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            Uc_Controls uC_Car = new(car);
                            CarsGroupbox.Children.Add(uC_Car);
                        });
                        Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                        Thread.Sleep(300);
                    }
                    stopwatch.Stop();

                    Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                }
            }
        }).Start();
    }
    public void MultiMode(CancellationToken cancellationToken)
    {
        var directory = new DirectoryInfo(@"..\..\..\Car Data");

        foreach (var item in directory.GetFiles())
        {

            ThreadPool.QueueUserWorkItem(state =>
            {
                stopwatch = Stopwatch.StartNew();
                if (item.Extension == ".json")
                {
                    var jsonText = File.ReadAllText(item.FullName);
                    var listCar = JsonSerializer.Deserialize<List<Car>>(jsonText);

                    if (listCar is null)
                        return;

                    foreach (var car in listCar)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            stopwatch.Stop();
                            Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                            break;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            Uc_Controls uC_Car = new(car);
                            CarsGroupbox.Children.Add(uC_Car);
                        });
                        Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                        Thread.Sleep(300);
                    }
                    stopwatch.Stop();

                    Dispatcher.Invoke(() => timeSpan.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:ff"));
                }
            });
        }
    }
    private void StartBtn_Click(object sender, RoutedEventArgs e)
    {

        if (Singlerdbtn.IsChecked == false && Multi.IsChecked == false)
        {
            MessageBox.Show("select 1 radio button");
            return;
        }
        if (Singlerdbtn.IsChecked==true)
        {
            cts = new();
            SingleMode(cts.Token);
        }
        if (Multi.IsChecked == true)
        {
            cts = new();
            MultiMode(cts.Token);
        }
    }
    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        cts?.Cancel();
        timeSpan.Text = "00:00:00:00";
    }
}
