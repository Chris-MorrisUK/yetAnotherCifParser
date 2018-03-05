using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess;
using System.Threading;
using VDS.RDF;
using System.Threading.Tasks;
using VDS.RDF.Parsing;
using System.IO;
using System.Globalization;
using ScheduleVis.BO;
using System.ComponentModel;

namespace ScheduleVis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowWithProgress
    {
        IGraph scheduleGraph;
        BackgroundWorker worker;

        public MainWindow()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += combinedImportWorker;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            StardogServerDetails serverDetails = frmStardogDetails.GetServerDetails();
            List<string> notUsed;
            if (serverDetails.Valid(out notUsed))
                System.Threading.Tasks.Task.Factory.StartNew(() => this.loadSchedule(serverDetails), CancellationToken.None, System.Threading.Tasks.TaskCreationOptions.AttachedToParent, System.Threading.Tasks.TaskScheduler.Current);
        }

        private object loadSchedule(StardogServerDetails serverDetails)
        {
            theDataSource = new StarDogLinkedDataSource(serverDetails);
            scheduleGraph = theDataSource.GetSparlAsGraph(Properties.Settings.Default.selectServices);
            populateServiceListFromGraph(scheduleGraph);
            // schedules = SchedulesCollection.CreateFromSparql(theDataSource.Query(Properties.Settings.Default.selectServices)) as SchedulesCollection;
            return null;

        }

        private void populateServiceListFromGraph(IGraph sourceGraph)
        {
            NodeFactory fact = new NodeFactory();
            IEnumerable<Triple> scheduledServices = sourceGraph.GetTriplesWithPredicateObject(UriNodeExt.RdfType(sourceGraph),
                fact.CreateUriNode(new Uri(Properties.Settings.Default.ScheduledService)));

            int rsSpaceTaken = RailService.RSWIDTH + RailService.RSSPACE;
            int nServices = scheduledServices.Count();
            uint nCol = (uint)Math.Floor(this.cnvServices.ActualWidth / (rsSpaceTaken)) - 1;
            uint nrows = (uint)nServices / nCol;
            uint col = 0; uint row = 0;
            double cnvHeight = this.cnvServices.ActualHeight;

            foreach (Triple service in scheduledServices)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    addRSDisplay(sourceGraph, rsSpaceTaken, nCol, ref col, ref row, service);
                }
                ));
            }
        }

        private void addRSDisplay(IGraph sourceGraph, int rsSpaceTaken, uint nCol, ref uint col, ref uint row, Triple service)
        {
            RailService rsToAdd = RailService.CreateFromNode(service.Subject as UriNode);
            cnvServices.Children.Add(rsToAdd);
            Canvas.SetLeft(rsToAdd, col * rsSpaceTaken);
            if (col++ > nCol)
            {
                col = 0;
                row++;
            }
            double top = row * rsSpaceTaken;
            Canvas.SetTop(rsToAdd, top);//allow a square space for now. This should probably be improved
            if (top + rsSpaceTaken > cnvServices.ActualHeight)
                cnvServices.Height = top + rsSpaceTaken;
        }



        private StarDogLinkedDataSource theDataSource
        {
            get
            {
                return ProgramState.TheDataSource;
            }
            set
            {
                ProgramState.TheDataSource = value;
            }
        }

        private void importStationList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.DefaultExt = ".msn";
            openDlg.Filter = "Master Station Name List (msn)|*.msn";
            openDlg.Title = "Station List to import";
            if (openDlg.ShowDialog(this) == true)
            {
                IFileController cntrl = new StationFileControl();
                FileParseBase parser = new FileParseBase();
                parser.MessageToDisplay += new FileParseBase.MessageDisplayDel(parser_MessageToDisplay);
                ProvInfo provInfo = new ProvInfo(txtName.Text, this.rbUri.IsChecked == true);
                List<Exception> Errors;
                IGraph stationNameGraph = parser.ParseFile(openDlg.FileName, provInfo, cntrl, out Errors,this, Properties.Settings.Default.Outputdir);
                saveGraphToTurtle(stationNameGraph);
                saveGraphToRDF(stationNameGraph);
            }

        }

        void parser_MessageToDisplay(string msg, string title, MessageBoxImage img)
        {
            displayMessage(msg, title, img);
        }

        private void saveGraphToTurtle(IGraph graphToSave)
        {
            Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
            saveDlg.DefaultExt = ".ttl";
            saveDlg.Filter = "Turtle Files|*.ttl";
            saveDlg.Title = "Output file";
            if (saveDlg.ShowDialog(this) == true)
            {
                graphToSave.SaveToFile(saveDlg.FileName);
            }
        }

        private void saveGraphToRDF(IGraph graphToSave)
        {
            Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
            saveDlg.DefaultExt = ".OWL";
            saveDlg.Filter = "OWL Files|*.OWL";
            saveDlg.Title = "Output file";
            if (saveDlg.ShowDialog(this) == true)
            {
                graphToSave.SaveToFile(saveDlg.FileName);
            }
        }



        private Uri generateTIPLOCUri(string tiploc)
        {
            string res = Properties.Settings.Default.ResourceBaseURI + tiploc;
            return UriFactory.Create(res);
        }





        //Saves invoking it every time, also allows for logging in the future, should it be deemed usefull
        private void displayMessage(string msg, string title, MessageBoxImage img)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(msg, title, MessageBoxButton.OK, img);
            }));
        }



        private void btnImportSchedules_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.DefaultExt = ".mca";
            openDlg.Filter = "Complete schedule file|*.mca";
            openDlg.Title = "Schedule to Import";
            if (openDlg.ShowDialog(this) == true)
            {
                IFileController cntrl = new ScheduleFileControl();
                FileParseBase parser = new FileParseBase();
                parser.MessageToDisplay += new FileParseBase.MessageDisplayDel(parser_MessageToDisplay);
                ProvInfo provInfo = new ProvInfo(txtName.Text, this.rbUri.IsChecked == true);
                List<Exception> Errors;
                IGraph stationNameGraph = parser.ParseFile(openDlg.FileName, provInfo, cntrl, out Errors,this, Properties.Settings.Default.Outputdir + "{0}_Schedules.ttl");
                saveGraphToTurtle(stationNameGraph);
            }
        }
        private struct CominedImportArgs
        {
            public CominedImportArgs(string stationName, string schedule, ProvInfo prov)
            {
                StationNameList = stationName;
                ScheduleFile = schedule;
                Prov = prov;               
            }
            public string StationNameList;
            public string ScheduleFile;
            public ProvInfo Prov;
            
        }
        private void btnCombinedImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.DefaultExt = ".msn";
            openDlg.Filter = "Master Station Name List (msn)|*.msn";
            openDlg.Title = "Station List to import";
            if (openDlg.ShowDialog(this) == true)
            {
                openDlg.DefaultExt = ".cif";
                openDlg.Filter = "Complete schedule file|*.cif";
                openDlg.Title = "Schedule to Import";
                string stationName = openDlg.FileName;
                if (openDlg.ShowDialog(this) == true)
                {
                    ProvInfo provInfo = null;
                    if (chkIncludeProv.IsChecked.Value)
                    {
                        provInfo = new ProvInfo(txtName.Text, this.rbUri.IsChecked == true);
                    }
                    CominedImportArgs toPass = new CominedImportArgs(stationName,openDlg.FileName,provInfo);                    
                    worker.RunWorkerAsync(toPass);
                }
            }
        }

        private void combinedImportWorker(object sender, DoWorkEventArgs e)
        {
            combinedImport(e.Argument);
        }

        private void combinedImport(object args)
        {
            IFileController cntrl = new StationFileControl();

            FileParseBase parser = new FileParseBase();
            parser.MessageToDisplay += new FileParseBase.MessageDisplayDel(parser_MessageToDisplay);
            
            List<Exception> Errors;
            IGraph combinedGraph = parser.ParseFile(((CominedImportArgs)args).StationNameList, ((CominedImportArgs)args).Prov, cntrl, out Errors,this, Properties.Settings.Default.Outputdir + "{0}_stations.ttl");
            IFileController scheduledCntrl = new ScheduleFileControl();
            List<Exception> ErrorsTwo;
            IGraph resultingGraph = parser.ParseFile(((CominedImportArgs)args).ScheduleFile, ((CominedImportArgs)args).Prov, scheduledCntrl, combinedGraph, out ErrorsTwo,this, Properties.Settings.Default.Outputdir + "{0}_callingPoints.ttl");
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    saveGraphToTurtle(resultingGraph);
            //}));
            //  saveGraphToRDF(resultingGraph);


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.DefaultExt = ".ttl";
            openDlg.Filter = "Turtle|*.ttl"; //|Owl|*.owl|All Files|*.*";
            openDlg.Title = "Graph to import";
            if (openDlg.ShowDialog(this) == true)
            {
                scheduleGraph = new Graph();
                TurtleParser parser = new TurtleParser();
                parser.Load(scheduleGraph, openDlg.FileName);
            }
        }

        /// <summary>
        /// Updates the progress bar
        /// </summary>
        /// <param name="fractionDoneOver255"> A number between 0 and 255 showing the current progress, within the range of byte
        /// /// </param>
        public void DoWorkStep(byte fractionDoneOver255)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (progress.Visibility == Visibility.Hidden)
                    progress.Visibility = Visibility.Visible;
                progress.Value = fractionDoneOver255;
            }));
        }

        public void DisplayMessage(string toDisplay)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (txtStatusMessages.Visibility == Visibility.Hidden)
                    txtStatusMessages.Visibility = Visibility.Visible;
                txtStatusMessages .Text = toDisplay;
            }));
        }

        public void Complete(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                txtStatusMessages.Visibility = Visibility.Hidden;
                progress.Visibility = Visibility.Hidden;
                MessageBox.Show(message, "Task Complete",MessageBoxButton.OK,MessageBoxImage.Information);
            }));
        }
    }
}
