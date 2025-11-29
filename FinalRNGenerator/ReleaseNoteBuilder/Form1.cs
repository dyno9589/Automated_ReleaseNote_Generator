using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace ReleaseNoteBuilder
{
    public partial class Form1 : Form
    {
        const int PreviewLines = 8;


        public Form1()
        {
            InitializeComponent();
        }


        private void btnBrowseBase_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select Base Release Folder (e.g. Release_4_0_4_10)";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtBaseFolder.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowseExport_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select Export Folder to save the RN .xlsx";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtExportFolder.Text = dlg.SelectedPath;
            }
        }





        private void btnBuildRN_Click(object sender, EventArgs e)
        {
            //txtBaseFolder.Text = "C:\\Users\\user\\Pictures\\New folder\\Exported RN\\Release_4_0_4_10";
            //txtExportFolder.Text = "D:\\RNBuildExport\\Export_31082025";

            string baseFolder = txtBaseFolder.Text.Trim();
            string exportFolder = txtExportFolder.Text.Trim();

            string clientName = cbClient.SelectedItem != null ? cbClient.SelectedItem.ToString() : string.Empty;
            string rlsCordintor = cbRNContr.SelectedItem != null ? cbRNContr.SelectedItem.ToString() : string.Empty;
            string releaseType = cbReleaseType.SelectedItem != null ? cbReleaseType.SelectedItem.ToString() : string.Empty;
            string dataPatch = cbDataptch.SelectedItem != null ? cbDataptch.SelectedItem.ToString() : string.Empty;

            // Validation checks
            if (string.IsNullOrWhiteSpace(clientName))
            {
                MessageBox.Show("Please select a Client.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(rlsCordintor))
            {
                MessageBox.Show("Please select a Release Coordinator.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(releaseType))
            {
                MessageBox.Show("Please select a Release Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(dataPatch))
            {
                MessageBox.Show("Please select a Data Patch option.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }




            lblStatus.Text = "Starting...";
            Application.DoEvents();

            if (string.IsNullOrEmpty(baseFolder) || !Directory.Exists(baseFolder))
            {
                MessageBox.Show("Please choose a valid Base Release folder.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatus.Text = "Cancelled - invalid base folder";
                return;
            }

            if (string.IsNullOrEmpty(exportFolder) || !Directory.Exists(exportFolder))
            {
                MessageBox.Show("Please choose a valid Export folder.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatus.Text = "Cancelled - invalid export folder";
                return;
            }


            try
            {
                var releaseName = new DirectoryInfo(baseFolder).Name;
                var outFile = Path.Combine(exportFolder, $"RN_{releaseName}_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx");

                lblStatus.Text = "Scanning files...";
                Application.DoEvents();

                var rows = BuildRows(baseFolder);

                lblStatus.Text = "Creating Excel...";
                Application.DoEvents();

                CreateXlsx(releaseName, baseFolder, rows, outFile);

                lblStatus.Text = $"Done: {outFile}";
                lblStatus.ForeColor = Color.Green;
                MessageBox.Show($"Release Note created:\n{outFile}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                MessageBox.Show("Error building release note:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Data row that will be written in Excel
        class RNRow
        {
            public string Database { get; set; }
            public string Schema { get; set; }
            public string ObjectType { get; set; }
            public string FileName { get; set; }
            public string FullPath { get; set; }
            public long SizeBytes { get; set; }
            public DateTime Modified { get; set; }
            public string ContentPreview { get; set; }
        }

        // Build the list of rows by walking the folder tree
        List<RNRow> BuildRows(string rootPath)
        {
            var rows = new List<RNRow>();
            var rootDir = new DirectoryInfo(rootPath);

            // Treat first-level directories as "databases" where possible
            var firstLevelDirs = rootDir.GetDirectories();

            foreach (var dbDir in firstLevelDirs)
            {
                string dbName = dbDir.Name;

                // get all files under this dbDir
                var files = dbDir.GetFiles("*.*", SearchOption.AllDirectories)
                                 .Where(f => !f.Extension.Equals(".meta", StringComparison.OrdinalIgnoreCase))
                                 .OrderBy(f => f.FullName);

                foreach (var f in files)
                {
                    // build path parts relative to dbDir
                    var relPath = f.FullName.Substring(dbDir.FullName.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var parts = relPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                    string schema = "(unknown)";
                    string objectType = "(unknown)";

                    // Heuristic:
                    // parts[0] = schema (if exists)
                    // parts[1] = objectType (if exists)
                    // otherwise use last folder before the file as objectType
                    if (parts.Length >= 3)
                    {
                        schema = parts[0];
                        objectType = parts[1];
                    }
                    else if (parts.Length == 2)
                    {
                        schema = parts[0];
                        objectType = "(files)";
                    }
                    else if (parts.Length == 1)
                    {
                        schema = "(root)";
                        objectType = "(root files)";
                    }
                    else
                    {
                        schema = "(root)";
                        objectType = "(root files)";
                    }

                    string preview = "";
                    if (IsTextFile(f.FullName) && PreviewLines > 0)
                    {
                        try
                        {
                            var lines = File.ReadAllLines(f.FullName);
                            preview = string.Join(Environment.NewLine, lines.Take(PreviewLines));
                            if (lines.Length > PreviewLines) preview += Environment.NewLine + "...";
                        }
                        catch
                        {
                            preview = "";
                        }
                    }

                    rows.Add(new RNRow
                    {
                        Database = dbName,
                        Schema = schema,
                        ObjectType = objectType,
                        FileName = f.Name,
                        FullPath = f.FullName,
                        SizeBytes = f.Length,
                        Modified = f.LastWriteTime,
                        ContentPreview = preview
                    });
                }
            }

            // Also capture files directly under rootPath (not inside a DB folder)
            var topFiles = rootDir.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                                   .Where(fi => !fi.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase));
            foreach (var tf in topFiles)
            {
                string preview = "";
                if (IsTextFile(tf.FullName) && PreviewLines > 0)
                {
                    try
                    {
                        var lines = File.ReadAllLines(tf.FullName);
                        preview = string.Join(Environment.NewLine, lines.Take(PreviewLines));
                        if (lines.Length > PreviewLines) preview += Environment.NewLine + "...";
                    }
                    catch { preview = ""; }
                }

                rows.Add(new RNRow
                {
                    Database = "(root)",
                    Schema = "(root)",
                    ObjectType = "(root files)",
                    FileName = tf.Name,
                    FullPath = tf.FullName,
                    SizeBytes = tf.Length,
                    Modified = tf.LastWriteTime,
                    ContentPreview = preview
                });
            }

            return rows;
        }

        static readonly string[] TextFileExtensions = new[]
        {
            ".sql", ".txt", ".xml", ".json", ".config", ".cs", ".js", ".css", ".html", ".htm", ".ps1", ".bat"
        };

        bool IsTextFile(string path)
        {
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext != null && TextFileExtensions.Contains(ext);
        }


        void CreateXlsx(string releaseName, string sourceFolder, List<RNRow> rows, string outFile)
        {

            string clientName = cbClient.SelectedItem.ToString();
            string rlsCordintor = cbRNContr.SelectedItem.ToString();
            string releaseType = cbReleaseType.SelectedItem.ToString();
            string dataPatch = cbDataptch.SelectedItem.ToString();
            string purposeRls = txtPrps.Text.ToString();
            string rootCause = txtRCA.Text.ToString();
            string resolution = txtResolution.Text.ToString();
            string impactRls = txtImpctRls.Text.ToString();
            string funImpact = txtFunImpct.Text.ToString();
            string modulesImpact = txtModuleImpct.Text.ToString();
            string deploymentSteps = txtDeploySteps.Text.ToString();
            string rollbckSteps = txtRollbckSteps.Text.ToString();
            if (rollbckSteps == "") rollbckSteps = "NA";
            if (deploymentSteps == "") deploymentSteps = "NA";
            if (modulesImpact == "") modulesImpact = "NA";
            if (funImpact == "") funImpact = "NA";
            if (impactRls == "") impactRls = "NA";
            if (resolution == "") resolution = "NA";
            if (rootCause == "") rootCause = "NA";
            if (purposeRls == "") purposeRls = "NA";


            using (var wb = new XLWorkbook())
            {
            // -------- Current Rel XLS --------
            var wsRel = wb.Worksheets.Add("Current Rel XLS");
            wsRel.Cell("A1").Value = "WINSOFT RELEASE NOTE";   // Winsoft RN
            wsRel.Cell("A2").Value = "Client";   // Client
            wsRel.Cell("C2").Value = clientName;   // Client Name
            wsRel.Cell("A3").Value = "Application Name";   // Application Name
            wsRel.Cell("C3").Value = "DeMATrix";   // Application Name
            wsRel.Cell("A4").Value = "Release ID";  // Release ID
            wsRel.Cell("C4").Value = releaseName;  // Release ID
            wsRel.Cell("A5").Value = "Release Date";  // Release Date
            wsRel.Cell("C5").Value = DateTime.Now.ToString("dd/MM/yyyy");
            wsRel.Cell("D4").Value = "Bug/ Enh. ID";
            wsRel.Cell("E4").Value = "<BugID>";
            wsRel.Cell("D5").Value = "Environment";
            wsRel.Cell("E5").Value = releaseType;
            wsRel.Cell("A6").Value = "Type of Release";
            wsRel.Cell("C6").Value = "PR/CR";
            wsRel.Cell("A7").Value = "Release Co-Ordinator";
            wsRel.Cell("C7").Value = rlsCordintor;
            wsRel.Cell("D6").Value = "Category of Release";
            wsRel.Cell("E6").Value = "Permanent";
            wsRel.Cell("A8").Value = "Release Deployment Steps";         // Deployment Steps
            wsRel.Cell("C8").Value = deploymentSteps;         // Deployment Steps
            wsRel.Cell("A9").Value = "Release Rollback Plan";         // Rollback Plan
            wsRel.Cell("C9").Value = rollbckSteps;         // Rollback Plan

            wsRel.Range("A1:H1").Merge();
            wsRel.Range("A2:B2").Merge();
            wsRel.Range("A3:B3").Merge();
            wsRel.Range("A4:B4").Merge();
            wsRel.Range("A5:B5").Merge();
            wsRel.Range("A6:B6").Merge();
            wsRel.Range("A7:B7").Merge();
            wsRel.Range("A8:B8").Merge();
            wsRel.Range("A9:B9").Merge();
            wsRel.Range("C2:H2").Merge();
            wsRel.Range("C3:H3").Merge();
            wsRel.Range("E4:H4").Merge();
            wsRel.Range("E5:H5").Merge();
            wsRel.Range("E6:H6").Merge();
            wsRel.Range("C7:H7").Merge();
            wsRel.Range("C8:H8").Merge();
            wsRel.Range("C9:H9").Merge();

            wsRel.Cell("C2").Style.Font.FontSize = 15;
            wsRel.Cell("C2").Style.Font.Bold = true;
            wsRel.Cell("C2").Style.Font.FontName = "Verdana";
            wsRel.Cell("C2").Style.Font.FontColor = XLColor.Orange;

            string basePath = AppDomain.CurrentDomain.BaseDirectory; // points to bin\Debug or bin\Release
            string imagePath = Path.Combine(basePath, "img", "WinsoftLogo.png");

            var picture = wsRel.AddPicture(imagePath);
            var cell = wsRel.Cell("H1");
            picture.MoveTo(cell).Scale(0.5);

            var width = wsRel.Column("H").Width;
            picture.MoveTo(wsRel.Cell("H1"), new System.Drawing.Point((int)(width * 7), 0));


            StyleDataTitleWinRN(wsRel.Range("A1:H1"));

            ApplyBorders(wsRel.Range("A1:H9"));
            StyleHeaderLeft(wsRel.Range(2, 1, 9, 1));
            StyleHeaderLeft(wsRel.Range(4, 4, 6, 4));
            
            wsRel.Range("A2:A9").Style.Font.FontColor = XLColor.Navy;
            wsRel.Range("D4:D6").Style.Font.FontColor = XLColor.Navy;
            wsRel.Cell("C3").Style.Font.Bold= true;
            wsRel.Cell("E5").Style.Font.Bold= true;
            wsRel.Cell("H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            wsRel.Range("C8:C9").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            wsRel.Columns("A").Width = 20;
            wsRel.Columns("H").Width = 23;
            wsRel.Row(1).Height= 30;
            wsRel.Row(8).Height= 100;
            wsRel.Row(9).Height= 50;

                //wsRel.Rows().AdjustToContents();
                wsRel.Columns("C:G").AdjustToContents();

            
        
            // -------- Change Details --------
            var wsChange = wb.Worksheets.Add("Change Details");
            wsChange.Cell("A1").Value = "Sr No";
            wsChange.Cell("A2").Value = "1";
            wsChange.Cell("B1").Value = "Bug / Enhancement ID ";
            wsChange.Cell("B2").Value = "";
            wsChange.Cell("C1").Value = "";
            wsChange.Cell("D1").Value = "Details";
            wsChange.Cell("E1").Value = "Developer Name";
            wsChange.Cell("E2").Value = rlsCordintor;
            wsChange.Cell("F1").Value = "QA Name";
            wsChange.Cell("G1").Value = "BA Name";
            wsChange.Cell("H1").Value = "Release Approver";
            wsChange.Cell("C2").Value = "Purpose Of Release *";
            wsChange.Cell("D2").Value = purposeRls;
            wsChange.Cell("C3").Value = "Root Cause Analysis *";
            wsChange.Cell("D3").Value = rootCause;
            wsChange.Cell("C4").Value = "Resolution *";
            wsChange.Cell("D4").Value = resolution;
            wsChange.Cell("C5").Value = "Data patch";
            wsChange.Cell("D5").Value = dataPatch;
            wsChange.Cell("C6").Value = "Impact of Release *";
            wsChange.Cell("D6").Value = impactRls;
            wsChange.Cell("C7").Value = "Functional Impacted *";
            wsChange.Cell("D7").Value = funImpact;
            wsChange.Cell("C8").Value = "Modules Impacted *";
            wsChange.Cell("D8").Value = modulesImpact;
            
            ApplyBorders(wsChange.Range("A1:H8"));
            wsChange.Cell("A2").Style.Font.FontColor = XLColor.Navy;
            wsChange.Range("C2:C8").Style.Font.FontColor = XLColor.Navy;
            wsChange.Range("A1:H1").Style.Font.Bold = true;
            wsChange.Range("A2:C8").Style.Font.Bold = true;
            wsChange.Range("A2:B2").Style.Fill.BackgroundColor = XLColor.LightGray;
            wsChange.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;
            wsChange.Range("C1:C8").Style.Fill.BackgroundColor = XLColor.LightGray;
            wsChange.Range("A2:B2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsChange.Range("A2:B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsChange.Column("A").Width = 6;
            wsChange.Column("B").Width = 20;
            wsChange.Column("C").Width = 22;
            wsChange.Column("D").Width = 80;

            wsChange.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            wsChange.Range("A2:A8").Merge();
            wsChange.Range("B2:B8").Merge();
            wsChange.Range("E2:E8").Merge();
            wsChange.Range("F2:F8").Merge();
            wsChange.Range("G2:G8").Merge();
            wsChange.Range("H2:H8").Merge();

            wsChange.Range("E2:H2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            wsChange.Columns("E:H").AdjustToContents();

            // -------- Object List --------
            var wsObj = wb.Worksheets.Add("Object List");
            wsObj.Cell("A1").Value = "WINSOFT RELEASE NOTE";
            wsObj.Cell("A2").Value = "Client *";
            wsObj.Cell("C2").Value = clientName;
            wsObj.Cell("A3").Value = "DEPLOYMENT DETAILS OF RELEASED OBJECTS";
            wsObj.Cell("D4").Value = "New CI details";
            wsObj.Cell("F4").Value = "Backup Required";
            wsObj.Cell("G4").Value = "Deployment Path";
            wsObj.Cell("J4").Value = "Objects Verified with production source";

            wsObj.Cell("C2").Style.Font.FontSize = 15;
            wsObj.Cell("C2").Style.Font.Bold = true;
            wsObj.Cell("C2").Style.Font.FontName = "Verdana";
            wsObj.Cell("C2").Style.Font.FontColor = XLColor.Orange ;

            wsObj.Range("A1:H1").Merge();
            wsObj.Range("A2:B2").Merge();
            wsObj.Range("C2:H2").Merge();
            wsObj.Range("A3:H3").Merge();

            wsObj.Cell("C2").Style.Font.FontSize = 15;
            wsObj.Cell("C2").Style.Font.Bold = true;
            wsObj.Cell("C2").Style.Font.FontName = "Verdana";
            wsObj.Cell("C2").Style.Font.FontColor = XLColor.Orange;

            string basePathObj = AppDomain.CurrentDomain.BaseDirectory; // points to bin\Debug or bin\Release
            string imagePathObj = Path.Combine(basePathObj, "img", "WinsoftLogoObj.png");

            var pictureObj = wsObj.AddPicture(imagePathObj);
            var cellObj = wsObj.Cell("H1");
            pictureObj.MoveTo(cellObj).Scale(0.5);

            var widthObj = wsObj.Column("H").Width;
            pictureObj.MoveTo(wsObj.Cell("H1"), new System.Drawing.Point((int)(widthObj * 10), 0));

            ApplyBorders(wsObj.Range("A1:K5"));
            wsObj.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsObj.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsObj.Cell("A3").Style.Font.FontColor = XLColor.Navy;
            wsObj.Cell("A3").Style.Font.FontSize = 12;
            wsObj.Cell("A3").Style.Font.Bold = true;
            wsObj.Cell("A2").Style.Font.Bold = true;
            wsObj.Range("A4:K5").Style.Font.Bold = true;
            wsObj.Cell("A3").Style.Font.FontName = "Arial";


            StyleDataTitleWinRN(wsObj.Range("A1:H1"));

            wsObj.Cell("A4").Value = "SR #";
            wsObj.Cell("B4").Value = "Interface / Object List *";
            wsObj.Cell("C4").Value = "Type of File *";
            wsObj.Cell("D4").Value = "New CI details";
            wsObj.Cell("F4").Value = "Backup \n Required *";
            wsObj.Cell("G4").Value = "Deployment Path";
            wsObj.Cell("J4").Value = "Objects Verified with production source";
            wsObj.Cell("D5").Value = "Owner";
            wsObj.Cell("E5").Value = "Date";
            wsObj.Cell("F5").Value = "Yes / No";
            wsObj.Cell("G5").Value = "Server *";
            wsObj.Cell("H5").Value = "Path/DB *";
            wsObj.Cell("I5").Value = "Code Reviewed By";
            wsObj.Cell("J5").Value = "Yes / No";
            wsObj.Cell("K5").Value = "Changes in Object\r\n(only expected or additional like Performance and functionality)";

            wsObj.Range("A4:A5").Merge();
            wsObj.Range("B4:B5").Merge();
            wsObj.Range("C4:C5").Merge();
            wsObj.Range("D4:E4").Merge();
            wsObj.Range("G4:H4").Merge();

            wsObj.Range("A3:K5").Style.Fill.BackgroundColor = XLColor.LightGray;
            wsObj.Cell("A2").Style.Fill.BackgroundColor = XLColor.LightGray;
            wsObj.Range("A3:K5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsObj.Range("A3:K5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsObj.Range("A3:K5").Style.Font.FontColor = XLColor.Navy;
            wsObj.Cell("A2").Style.Font.FontColor = XLColor.Navy;

            wsObj.Columns("A").Width = 6;
            wsObj.Columns("C").Width = 15;
            wsObj.Columns("D").Width = 22;
            wsObj.Columns("E").Width = 18;
            wsObj.Columns("F").Width = 12;
            wsObj.Columns("G").Width = 25;
            wsObj.Columns("H").Width = 45;
            wsObj.Row(1).Height = 30;
            wsObj.Row(4).Height = 20;


            var headers = new[]
            {
                "SR#", "Interface / Object List *", "Type of File *", "Owner", "Date",
                "Backup Required *", "Server *", "Path/DB *",
                "Code Reviewed By", "Objects Verified", "Changes in Object"
            };


                // ---------- Replace from here ----------

                // Define database file extensions (SQL Server + common DB-related)
                string[] dbExtensions = {
                    ".sql",".sqlplan", ".pls", ".pkb", ".pks", ".trg",
                    ".fnc", ".prc", ".vw", ".db", ".dmp", ".bak",".mdf", ".ndf", ".ldf", ".trn",
                    ".tuf",".tab"                };

                // Define ignored extensions
                string[] ignoreExtensions = { ".bat", ".ini" };

                // Split Application vs Database and order by full path so sibling files are grouped together
                var dbRows = rows
                    .Where(r => dbExtensions.Contains(Path.GetExtension(r.FileName), StringComparer.OrdinalIgnoreCase)
                                && !ignoreExtensions.Contains(Path.GetExtension(r.FileName), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(r => r.FullPath)
                    .ToList();

                var appRows = rows
                    .Where(r => !dbExtensions.Contains(Path.GetExtension(r.FileName), StringComparer.OrdinalIgnoreCase)
                                && !ignoreExtensions.Contains(Path.GetExtension(r.FileName), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(r => r.FullPath)
                    .ToList();

                int roww = 7;

                // ---- APPLICATION SECTION ----
                if (appRows.Any())
                {
                    wsObj.Cell(roww, 2).Value = "APPLICATION";
                    wsObj.Range(roww, 2, roww, headers.Length).Merge();
                    wsObj.Range(roww, 1, roww, headers.Length).Style.Fill.BackgroundColor = XLColor.Green;
                    wsObj.Range(roww, 1, roww, headers.Length).Style.Font.Bold = true;
                    roww++;

                    // track printed folder relative paths so each folder is printed once
                    var printedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    // track per-folder file counters to restart SR# per folder
                    var folderFileIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                    foreach (var item in appRows)
                    {
                        // relativePath is relative to the base release folder (sourceFolder param)
                        var relativePath = GetRelativePath(sourceFolder, item.FullPath).TrimStart(Path.DirectorySeparatorChar);
                        var parts = relativePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0) continue;

                        // build folder accumulation step-by-step (everything except the last part which is file)
                        string folderAccum = "";
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            folderAccum = folderAccum == "" ? parts[i] : folderAccum + Path.DirectorySeparatorChar + parts[i];

                            if (!printedFolders.Contains(folderAccum))
                            {
                                // print folder row (only name, indented by depth)
                                wsObj.Cell(roww, 2).Value =  parts[i];
                                wsObj.Cell(roww, 2).Style.Font.Bold = true;
                                wsObj.Cell(roww, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                                //wsObj.Cell(roww, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                                //wsObj.Cell(roww, 1).Value = ""; // SR# blank for folder
                                printedFolders.Add(folderAccum);

                                wsObj.Cell(roww, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                wsObj.Cell(roww, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                roww++;
                            }
                        }

                        // Now print the file row under its parent folder
                        string parentKey = folderAccum;
                        if (string.IsNullOrEmpty(parentKey)) parentKey = "(root)";

                        int fileIndex;
                        if (folderFileIndex.TryGetValue(parentKey, out int cur)) { cur++; folderFileIndex[parentKey] = cur; fileIndex = cur; }
                        else { folderFileIndex[parentKey] = 1; fileIndex = 1; }

                        wsObj.Cell(roww, 1).Value = fileIndex; // SR# per-folder
                        wsObj.Cell(roww, 2).Value = parts.Last(); // file name
                        wsObj.Cell(roww, 3).Value = Path.GetExtension(item.FileName).TrimStart('.');
                        wsObj.Cell(roww, 4).Value = "DEV TEAM";
                        wsObj.Cell(roww, 5).Value = item.Modified.ToString("dd/MM/yyyy"); // prefer file's modified date
                        wsObj.Cell(roww, 6).Value = "Yes";
                        wsObj.Cell(roww, 7).Value = "APPLICATION SERVER";
                        //wsObj.Cell(roww, 8).Value = item.Schema;
                        wsObj.Cell(roww, 8).Value = parentKey.ToString();


                        wsObj.Cell(roww, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wsObj.Cell(roww, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        
                        wsObj.Cell(roww, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        
                        wsObj.Cell(roww, 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        
                        wsObj.Cell(roww, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        roww++;
                    }
                }

                // ---- DATABASE SECTION ----

                if (dbRows.Any())
                {
                    roww++;
                    wsObj.Cell(roww, 2).Value = "DATABASE";
                    wsObj.Range(roww, 2, roww, headers.Length).Merge();
                    wsObj.Range(roww, 1, roww, headers.Length).Style.Fill.BackgroundColor = XLColor.Green;
                    wsObj.Range(roww, 1, roww, headers.Length).Style.Font.Bold = true;
                    roww++;

                    var printedFoldersDb = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var folderFileIndexDb = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                    foreach (var item in dbRows)
                    {
                        //temp
                        //string folderNameDBPath = Path.GetFileName(Path.GetDirectoryName(item.FullPath));

                        var relativePath = GetRelativePath(sourceFolder, item.FullPath).TrimStart(Path.DirectorySeparatorChar);
                        var parts = relativePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0) continue;

                        string folderAccum = "";
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            folderAccum = folderAccum == "" ? parts[i] : folderAccum + Path.DirectorySeparatorChar + parts[i];

                            if (!printedFoldersDb.Contains(folderAccum))
                            {
                                wsObj.Cell(roww, 2).Value =  parts[i];
                                wsObj.Cell(roww, 2).Style.Font.Bold = true;
                                wsObj.Cell(roww, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                                wsObj.Cell(roww, 1).Value = "";
                                printedFoldersDb.Add(folderAccum);
                                roww++;
                            }
                        }

                        string parentKey = folderAccum;
                        if (string.IsNullOrEmpty(parentKey)) parentKey = "(root)";

                        int fileIndex;
                        if (folderFileIndexDb.TryGetValue(parentKey, out int cur)) { cur++; folderFileIndexDb[parentKey] = cur; fileIndex = cur; }
                        else { folderFileIndexDb[parentKey] = 1; fileIndex = 1; }

                        wsObj.Cell(roww, 1).Value = fileIndex;
                        wsObj.Cell(roww, 2).Value = parts.Last();
                        wsObj.Cell(roww, 3).Value = Path.GetExtension(item.FileName).TrimStart('.');
                        wsObj.Cell(roww, 4).Value = "DATABASE TEAM";
                        wsObj.Cell(roww, 5).Value = item.Modified.ToString("dd/MM/yyyy");
                        wsObj.Cell(roww, 6).Value = "Yes";
                        wsObj.Cell(roww, 7).Value = "DATABASE SERVER";
                        //wsObj.Cell(roww, 8).Value = item.Schema;

                        if (dbRows.Any())
                        {
                            string schemaValue = "";
                            string lowerFolder = relativePath.ToLower();

                            if (lowerFolder.Contains("nsdl"))
                                schemaValue = "edpbos_nsdl";
                            else if (lowerFolder.Contains("cdsl"))
                                schemaValue = "edpbos_cdsl";
                            else if (lowerFolder.Contains("sys"))
                                schemaValue = "edpbos_sys";
                            else if (lowerFolder.Contains("dissim"))
                                schemaValue = "dematrix_dissim";
                            else if (lowerFolder.Contains("email"))
                                schemaValue = "dematrix_email";

                            wsObj.Cell(roww, 8).Value = schemaValue;
                        }
                        else
                        {
                            wsObj.Cell(roww, 8).Value = item.Schema;
                        }

                        wsObj.Cell(roww, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        
                        wsObj.Cell(roww, 2).Style.Alignment.Vertical= XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 2).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Left;

                        wsObj.Cell(roww, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsObj.Cell(roww, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        wsObj.Cell(roww, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        roww++;
                    }
                }

                // ---------- Replace to here ----------



            //int lastRow = wsObj.LastRowUsed().RowNumber();
            //ApplyBorders(wsObj.Range($"A1:K{lastRow}"));

                StyleData(wsObj.Range(6, 1, roww - 1, headers.Length));
            wsObj.Column("B").AdjustToContents();
            wsObj.Columns("I:K").AdjustToContents();

            // -------- Exception Details --------
            var wsExc = wb.Worksheets.Add("Exception Details");
            wsExc.Cell(1, 1).Value = "Sr No.";
            wsExc.Cell(1, 2).Value = "Object name";
            wsExc.Cell(1, 3).Value = "Object Type";
            wsExc.Cell(1, 4).Value = "Purpose";
            wsExc.Cell(1, 5).Value = "Remark";
            
            wsExc.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsExc.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsExc.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsExc.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsExc.Column("B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsExc.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsExc.Column("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            wsExc.Column("A").Width = 7;
            wsExc.Column("B").Width = 70;
            wsExc.Column("C").Width = 15;
            wsExc.Column("D").Width = 45;
            wsExc.Column("E").Width = 45;

                StyleHeader(wsExc.Range(1, 1, 1, 5));


            wsExc.Cell(2, 1).Value = 1;
            wsExc.Cell(2, 2).Value = "NA";
            wsExc.Cell(2, 3).Value = "NA";
            wsExc.Cell(2, 4).Value = "NA";
            wsExc.Cell(2, 5).Value = "NA";

            StyleData(wsExc.Range(2, 1, 2, 5));
            //wsExc.Columns().AdjustToContents();

            // -------- Basic Check list --------
            var wsChk = wb.Worksheets.Add("Basic Check list");
            string[] checks =
            {
                "Procedure should be enclosed within BEGIN...END (Try catch)",
                "First 2 lines of the proc should read \n SET NOCOUNT ON; \n SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED",
                "All Stored Procedures should have dbo. to qualify the schema while using tables",
                "All DDL statements should be at the beginning of the stored procedure",
                "Stored Procedures should NOT have any select * statement, it should be explecit \n (there is no exception to this be it # @ or Physical table)",
                "Each table to have primary key, irrespective of its temporary physical, getting truncated.",
                "All columns which are used in stored procedure in \"Where\", \"Order BY\" , \"Group BY\" should have proper indexes created.",
                "There no statement like \"select @Variable = <Field> from <table> where <condition OR without Condiation>. Do NOT use non ANSI statement.\r\n   Set is ANSI statement to assign values to variable; select is NOT.",
                "Confirm there are no cursors / loops while huge data processing.",
                "There are no #tables created without columns specified",
                "There are no alter statements in Stored Procedure on a table created as #temp "
            };
            wsChk.Cell(1, 1).Value = "Sr No";
            wsChk.Cell(1, 2).Value = "Steps";
            wsChk.Cell(1, 3).Value = "Flag";
            wsChk.Cell(1, 4).Value = "Remarks";

            wsChk.Column("A").Width = 5;
            wsChk.Column("B").Width = 128;
            wsChk.Column("C").Width = 10;
            wsChk.Column("D").Width = 40;

            wsChk.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wsChk.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            StyleHeader(wsChk.Range(1, 1, 1, 4));

            for (int i = 0; i < checks.Length; i++)
            {
                wsChk.Cell(i + 2, 1).Value = i + 1;
                wsChk.Cell(i + 2, 2).Value = checks[i];
            }

            ApplyBorders(wsChk.Range("C2:D12"));
            StyleData(wsChk.Range(2, 1, checks.Length + 1, 2));
            //wsChk.Columns().AdjustToContents();


            // Save
            wb.SaveAs(outFile);
        }
}

        private void StyleHeader(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Fill.BackgroundColor = XLColor.LightGray;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private void StyleHeaderLeft(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Fill.BackgroundColor = XLColor.LightGray;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

    //Optional method
    //public static string GetRelativePath(string basePath, string fullPath)
    //{
    //    Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
    //    Uri fullUri = new Uri(fullPath);

    //    return Uri.UnescapeDataString(
    //        baseUri.MakeRelativeUri(fullUri).ToString()
    //    ).Replace('/', Path.DirectorySeparatorChar);
    //}

    public static string GetRelativePath(string basePath, string fullPath)
    {
        Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
        Uri fullUri = new Uri(fullPath);

        return Uri.UnescapeDataString(
            baseUri.MakeRelativeUri(fullUri).ToString()
                   .Replace('/', Path.DirectorySeparatorChar));
    }

    //private static string AppendDirectorySeparatorChar(string path)
    //    {
    //        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
    //            return path + Path.DirectorySeparatorChar;
    //        return path;
    //    }

    private static string AppendDirectorySeparatorChar(string path)
    {
        // Ensures the base path always ends with a separator
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            return path + Path.DirectorySeparatorChar;

        return path;
    }

    private void StyleData(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private void StyleDataTitleWinRN(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Font.Bold= true;
            range.Style.Font.Underline = XLFontUnderlineValues.Single;
            range.Style.Font.FontName = "Arial";
            range.Style.Font.FontSize = 20;
            range.Style.Font.FontColor = XLColor.Orange;

        }



        private void ApplyBorders(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }



    }
}

