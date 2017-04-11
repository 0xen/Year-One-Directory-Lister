// Student ID: 20713365
// Date 20/01/2017

/* Extra Features
    - Ability to execute files from application: Navigate into file listing, select file number and if it is a '.exe' file then you can press '1'
      to execute it.
    - File Size Suffixes: When information about the files are listed (For example in the FullFileListing) the appropriate file size suffix will be
      applied to the file.
    - Generating Tree Structure: Based on the directory you are currently in, in the main menu if you press option '5'(Generate Directory Tree) you
      will get a entire tree structure of the children files and folders outputted onto you're desktop. This will only work for files that windows
      does not have read protection on for.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace FileListingAndSearching
{
    class Program
    {
        //// Global Variables ////
        static string CurrentDirectory;                       // This will store the current directory that is being accessed by the program
        static string PreInputPrefix;                         // This variable will store the character sequence that will be outputted before the users string input "EG:* > *[User Input]"

        //// Sorting ////
        static bool ASC;                                      // This boolean stores if we are sorting ascending or descending
        static string[] SortDefinitions;                      // Here we store the name representations of the sorting methods EG: "Sort By Name"
        static int SortType;                                  /* Here we store the integer value of the sort type
                                                                    0: Default Order
                                                                    1: Sort by Name
                                                                    2: Sort by Size
                                                                    3: Sort by Creation Date
                                                                    4: Sort by Last Accessed
                                                                    5: Sort by Extension
                                                              */

        //// Global Messages ////
        static string OutOfBoundsMessage;                     // This is the message that will be outputted when the value is out of bounds of the minimum and maximum values
        static string InvalidUserInputMessageWithRangeMinMax; // This message outputs the expected input type of the console input and what range the value needs to be in with a minimum and maximum
        static string InvalidUserInputMessageWithRangeMin;    // This message outputs the expected input type of the console input and what range the value needs to be in with a minimum
        static string FileListingMessage;                     // This is the message that will be outputted when displaying the file listings

        //// Extra Data ////
        static string[] FileSizeSuffixs;                      // This contains all the suffix's for file sizes, etc GB
        static List<string> TempTreeStructure;                // When Generating the tree structure, this is the file that will hold the files lines which will be outputted into the file



        /// <summary>
        ///     Initializes all global variables for the application. Should only be done on boot.
        /// </summary>
        static void initilization()
        {
            CurrentDirectory = "C:\\Windows"; // Initializes the folder in-which the program will be focusing on.
            PreInputPrefix = "> "; // Initializes the prefix that will be used as a user input cursor

            ASC = true; // Defines if 'when' sorting the results if its sorting by ascending or not
            SortType = 0; /* Defined the way that files will be sorted
                            0: Default Order
                            1: Sort by Name
                            2: Sort by Size
                            3: Sort by Creation Date
                            4: Sort by Last Accessed
                            5: Sort by Creation
                          */
            SortDefinitions = new string[6] { // Defines the text representations of the sorting orders.
                "Default",
                "Name",
                "Size",
                "Creation Time",
                "Last Accessed",
                "Extension"
            };

            // Out of bounds message for when a user types in a invalid integer
            OutOfBoundsMessage = "Out of bounds, expecting range from {0}-{1}";
            // Message used for when a user input is not the correct type or is not in range
            InvalidUserInputMessageWithRangeMinMax = "Invalid input, expecting [{0}] in range of [{1}-{2}]";
            // Message used for when a user input is not the correct type or is not in range
            InvalidUserInputMessageWithRangeMin = "Invalid input, expecting [{0}] grater then or equal to [{1}]";
            // This is the message that will be outputted when displaying the file listings
            FileListingMessage = "{0}. {1} ({2}) [{3}]";

            // Initializes an array of the 5 most commonly used file size suffix's
            FileSizeSuffixs = new string[5] {
                "Bytes",
                "KB",
                "MB",
                "GB",
                "TB"
            };
            // Initializes a temporary List which will store the output file contents for the tree structure
            TempTreeStructure = new List<string>();
        }

        /// <summary>
        ///     Used to create a blank line in the console
        /// </summary>
        static void ConsolePause()
        {
            Console.ReadLine(); // Outputs a single blank line
        }

        /// <summary>
        ///     Output a message with and replaces instances of {i} in the message string with
        ///     the value that is index the same in the "messageParameters" array.
        /// </summary>
        /// <param name="message">
        ///     Message that will be outputted to the console. All instances of {i} will be replaced 
        ///     with the value that is index the same in the "messageParameters" array.
        /// </param>
        /// <param name="messageParameters">Contains the values that will replace all instances of {i} in the 'message' string.</param>
        static void OutputMessageWithParameters(string message, params object[] messageParameters)
        {
            Console.WriteLine(message, messageParameters); // Outputs the defined message with the relevant parameters
        }

        /// <summary>
        ///     This function outputs the value of 'PreInputPrefix' onto the current line (Acting as a cursor) and if 'consolePadding' 
        ///     is true then we add a new line before the cursor as padding.
        /// </summary>
        /// <param name="consolePadding">Boolean to choose if you want a new line before the 'PreInputPrefix' or not.</param>
        static void DisplayPreInputPrefix(bool consolePadding = false)
        {
            if (consolePadding) Console.WriteLine(); // Outputs a single black line if 'consolePadding' is true
            Console.Write(PreInputPrefix); // Outputs a prefix used before the user types in data
        }

        /// <summary>
        ///     Console function that outputs a cursor to the screen and then reads the input string from the user and returns it.
        /// </summary>
        /// <returns>Returns a string that the user types into the console.</returns>
        static string GetStringFromConsole()
        {
            DisplayPreInputPrefix(); // Output a prefix before we take the user's input
            return Console.ReadLine(); // Read in data from the console
        }

        /// <summary>
        ///     Console function that takes in a minimum and maximum allowed input from the user and keeps looping until
        ///     the range condition is met.
        /// </summary>
        /// <param name="min">Minimum allowed inputed number into the console</param>
        /// <param name="max">Maximum allowed inputed number into the console</param>
        /// <returns>Returns the user inputed integer within the range specified</returns>
        static int GetIntegerFromConsole(int min, int max)
        {
            int result; // Define a variable to store the response that the user will type in
            DisplayPreInputPrefix(true); // Output a prefix before we take the user's input
            while (
                !int.TryParse(Console.ReadLine(), out result) || // Get input from the user and try and parse it to a Integer
                result > max || // Check to see if the result is bigger then maximum
                result < min) // Check to see if the result is less then the minimum
            {
                // Outputs a error message and specifies the required variable type, minimum and maximum value.
                OutputMessageWithParameters(InvalidUserInputMessageWithRangeMinMax, typeof(int), min, max);
                DisplayPreInputPrefix(); // Output a prefix before we take the user's input
            }
            return result;
        }

        /// <summary>
        ///     Console function that takes in a minimum allowed input from the user and keeps looping until
        ///     the range condition is met.
        /// </summary>
        /// <param name="min">Minimum allowed inputed number into the console</param>
        /// <returns>Returns the user inputed integer within the range specified</returns>
        static int GetIntegerFromConsole(int min)
        {
            int result; // Define a variable to store the response that the user will type in
            DisplayPreInputPrefix(true); // Output a prefix before we take the user's input
            while (
                !int.TryParse(Console.ReadLine(), out result) || // Get input from the user and try and parse it to a Integer
                result < min) // Check to see if the result is less then the minimum
            {
                // Outputs a error message and specifies the required variable type, minimum and maximum value.
                OutputMessageWithParameters(InvalidUserInputMessageWithRangeMin, typeof(int), min);
                DisplayPreInputPrefix(); // Output a prefix before we take the user's input
            }
            return result;
        }

        /// <summary>
        ///     Here we output information about the file passed through to it such as; Name, Size, Creation Date, Etc.
        /// </summary>
        /// <param name="file"> File we want information on</param>
        static void DisplayFileInformation(FileInfo file)
        {
            //Outputs information regarding the file passed to the function
            OutputMessageWithParameters("File: {0}", file.Name);
            OutputMessageWithParameters("Full name: {0}", file.FullName);
            OutputMessageWithParameters("File Size: {0}", file.Length);
            OutputMessageWithParameters("Created: {0}", file.CreationTime);
            OutputMessageWithParameters("Last Accessed: {0}", file.LastAccessTime);
        }

        /// <summary>
        ///     When this function is called it displays the main menu options. Upon the user entering a value within the 
        ///     defined range, then user will be redirect.
        /// </summary>
        static void MainMenu()
        {
            int rangeMin = 1; // Minimum allowed user input
            int rangeMax = 9; // Maximum allowed user input

            // Outputs the main menu screen tot he console
            Console.WriteLine("Welcome to the file lister!");
            Console.WriteLine();
            Console.WriteLine("Current Directory {0}", CurrentDirectory);
            Console.WriteLine();
            Console.WriteLine("Enter a value between [{0}-{1}]", rangeMin, rangeMax);
            Console.WriteLine();
            Console.WriteLine("[1] Full File Listing");
            Console.WriteLine("[2] Filtered File Listing");
            Console.WriteLine("[3] File Size Listing");
            Console.WriteLine("[4] Recently Accessed File Listing");
            Console.WriteLine("[5] Generate Directory Tree");
            Console.WriteLine("[6] Folder Statistics");
            Console.WriteLine("[7] Change Directory");
            Console.WriteLine("[8] Change Sort");
            Console.WriteLine("[9] Exit");

            // Get the input from the user within the specified range and passes it to the switch case
            switch (GetIntegerFromConsole(rangeMin, rangeMax))
            {
                case 1: // Full File Listing
                    Console.Clear(); // Clear console
                    FullFileListingMenu(); // Open Full File Listing Screen
                    break;
                case 2: // Filtered File Listing
                    Console.Clear(); // Clear console
                    FilterdFileListingMenu(); // Open Filter File Listing Screen
                    break;
                case 3: // File Size Listing
                    Console.Clear(); // Clear console
                    FileSizeListingMenu(); // Open File Size Listing Screen
                    break;
                case 4: // Recently Accessed File Listing
                    Console.Clear(); // Clear console
                    DatedFileListingMenu(); // Open Dated File Listing Screen
                    break;
                case 5: // Generate Directory Tree
                    Console.Clear(); // Clear console
                    DirectoryTree(); // Creates a Directory Tree And Outputs It Into A File On The Desktop
                    break;
                case 6: // Folder Statistics
                    Console.Clear(); // Clear console
                    FolderStatistics(); // Opens Folder Statistics Based On 'CurrentDirectory'
                    break;
                case 7: // Change Directory
                    Console.Clear(); // Clear console
                    ChangeDirectory(); // Opens A Screen That Allows The User To Change Directory
                    break;
                case 8: // Change Sort
                    Console.Clear(); // Clear console
                    ChangeSort(); // Opens The Change Sort Order Screen
                    break;
                case 9: // Exit Application
                    return;
                default: // 
                    // Outputs a error message with a minimum and maximum range
                    OutputMessageWithParameters(OutOfBoundsMessage, rangeMin, rangeMax);
                    Console.Clear(); // Clear console
                    MainMenu(); // Loops back to the start of the main menu file
                    break;
            }
        }

        /// <summary>
        ///     Here we pull 'all' files inside a directory and pass them to 'FileListingMenu' to be displayed on the screen
        /// </summary>
        static void FullFileListingMenu()
        {
            // Gets information on the current directory
            DirectoryInfo folderInfo = new DirectoryInfo(CurrentDirectory);
            FileInfo[] files = folderInfo.GetFiles(); // Gets all files in the current directory and stores it in a array
            FileListingMenu(files); // Passes all the files onto the 'FileListingMenu' to be outputted
        }

        /// <summary>
        ///     Here we pull files that follow the specified filter, inside a directory and pass them to 'FileListingMenu' to be displayed on the screen.
        /// </summary>
        static void FilterdFileListingMenu()
        {
            Console.WriteLine("Please Enter a Valid Filter, e.g. *.exe"); // Output basic example of a valid filter to the user
            string filter = GetStringFromConsole(); // Get the users input that we will use for the file filter
            Console.Clear(); // Clear the console
            // Gets information on the current directory
            DirectoryInfo folderInfo = new DirectoryInfo(CurrentDirectory);
            FileInfo[] files = folderInfo.GetFiles(filter); // Gets all files in the current directory based on the filter and stores it in a array
            FileListingMenu(files); // Passes all the files onto the 'FileListingMenu' to be outputted
        }

        /// <summary>
        ///     Here we pull files within a 1 MegaByte range. The user will input the required size and passes it on to 'FileListingMenu' to be displayed
        /// </summary>
        static void FileSizeListingMenu()
        {
            int minimumValue = 0; // Define what the minimum value the user can enter is.
            Console.WriteLine("Please Enter a Valid Size In MB, e.g. 4"); // Output a example to the user of what file size input we are expecting
            int size = GetIntegerFromConsole(minimumValue); // Pulls integer value from the console equal too or over the minimum value
            Console.Clear(); // Clear the console
            // Gets information on the current directory
            DirectoryInfo folderInfo = new DirectoryInfo(CurrentDirectory);
            FileInfo[] allFilesInDirectory = folderInfo.GetFiles(); // Gets all files in the current directory and stores it in a array
            List<FileInfo> sizedFiles = new List<FileInfo>(); // Creates a list to hold the files that fall in the size range
            int midRange = (size * 1024) * 1024; // Gets the users input and works out how many bytes are in 'x' megabytes
            int sizeOfMBInBytes = 1024 * 1024; // Gets the amount of bytes in 1 megabyte
            foreach (FileInfo file in allFilesInDirectory) // Loops through all the files
            {
                if (file.Length > midRange - sizeOfMBInBytes && // Checks to see if the file length is larger then the 'midRange' - 'sizeOfMBInBytes'
                    file.Length < midRange + sizeOfMBInBytes) // Checks to see if the file length is smaller then the 'midRange' + 'sizeOfMBInBytes'
                {
                    sizedFiles.Add(file); // Append the file to the temporary array
                }
            }
            FileListingMenu(sizedFiles.ToArray()); // Passes all the files onto the 'FileListingMenu' to be outputted
        }

        /// <summary>
        ///     Here we get all files within a one month date range and will pass them onto 'FileListingMenu'
        /// </summary>
        static void DatedFileListingMenu()
        {
            // Gets information on the current directory
            DirectoryInfo folderInfo = new DirectoryInfo(CurrentDirectory);
            FileInfo[] allFilesInDirectory = folderInfo.GetFiles(); // Gets all files in the current directory and stores it in a array
            List<FileInfo> datedFiles = new List<FileInfo>(); // Creates a list to hold the files that fall in the date range
            foreach (FileInfo file in allFilesInDirectory) // Loops through all the files
            {
                if (file.LastAccessTime > DateTime.Now.AddMonths(1)) // Check to see if the last accessed time of the file is less then one moth ago
                {
                    datedFiles.Add(file); // Append the file to the temporary array
                }
            }
            FileListingMenu(datedFiles.ToArray()); // Passes all the files onto the 'FileListingMenu' to be outputted
        }

        /// <summary>
        ///     Here we create a formated string to be saved into a file creating a tree structure.
        ///     It takes in 2 parameters; size and isFolder. Level is the current folder level that the function is drawing the branch from
        /// </summary>
        /// <param name="level">Level is the current folder level that the function is drawing from</param>
        /// <param name="isFolder">isFolder defines if the branch being drawn for is a folder or a file</param>
        /// <returns>Returns the string that will be outputted into the tree file</returns>
        static string CreateTreeBranch(int level, bool isFolder)
        {
            string branch = ""; // Create a empty string to store the branch that will be outputted
            for (int i = 0; i < level; i++) // Loop through for the current branch level
            {
                // Here we append a string based on
                /* 
                 * # If 'i' does not equal 'level' - 1 then append "|   "
                 * # If 'i' dose equal 'level' - 1 then
                 *      # If isFolder equal true then we append "+--"
                 *      or
                 *      # We append |--
                 * 
                 * @ Doing this make a nicely formated file where you can clearly see where files and folders start and end
                 */
                branch += (i == level - 1 ? (isFolder ? "+--" : "|--") : "|   ");
            }
            return branch; // Return the current branch
        }

        /// <summary>
        ///     This function gets the current directory that is being processed to be outputted into the tree file and loops through all
        ///     files and folders in it, processing each in turn.
        /// </summary>
        /// <param name="directory">
        ///     Current directory that is being processed. Can be null and will start processing from the
        ///     value in 'CurrentDirectory'
        /// </param>
        /// <param name="level">Here we get the current level being processed</param>
        static void DrawDirectoryTree(DirectoryInfo directory = null, int level = 1)
        {
            if (directory == null) // Check to see if the directory is null. This will only happen if we have not passed any data to it
            {
                directory = new DirectoryInfo(CurrentDirectory); // Gets information on the current directory
                TempTreeStructure.Add(CreateTreeBranch(level - 1, true) + CurrentDirectory); // Pass the new branch to the global variable 'TempTreeStructure'
            }
            else
            {
                TempTreeStructure.Add(CreateTreeBranch(level - 1, true) + directory.Name); // Pass the new branch to the global variable 'TempTreeStructure'
            }
            try // Use a Try catch in case the file/folder we try and access is protected in windows
            {
                FileInfo[] allFilesInDirectory = directory.GetFiles(); // Gets all files in the current directory and stores it in a array
                for (int i = 0; i < allFilesInDirectory.Length; i++) // Loop through all files in the 'directory'
                {
                    FileInfo file = allFilesInDirectory[i]; // Get the current file
                    TempTreeStructure.Add(CreateTreeBranch(level, false) + file.Name); // Pass the new branch to the global variable 'TempTreeStructure'
                }
                DirectoryInfo[] allFoldersInDirectory = directory.GetDirectories(); // Gets all directories in the current directory and stores it in a array

                for (int i = 0; i < allFoldersInDirectory.Length; i++) // Loop through all directories in the 'directory'
                {
                    DirectoryInfo subDirectory = allFoldersInDirectory[i]; // Get the current directory
                    DrawDirectoryTree(subDirectory, level + 1); // Call DrawDirectoryTree to process the next branch of the tree
                }
            }
            catch (UnauthorizedAccessException) // Use a Try catch in case the file/folder we try and access is protected in windows
            {

            }
        }

        /// <summary>
        ///     This function is called to initiate creating a tree structure output of the 'CurrentDirectory' folder.
        /// </summary>
        static void DirectoryTree()
        {
            // Output the user a message asking them if there sure they want to proceed
            Console.WriteLine();
            Console.WriteLine("Processing The Tree Structure May Take A While, Do You Want To Continue?");
            Console.Write("[Y/N]? : ");
            string answer = GetStringFromConsole().ToLower(); // Here we get the users input to lower case
            Console.Clear(); // Clears the console
            if (answer == "y" || answer == "yes") // We check to see if the user agreed to preforming the action or not
            {
                // Gets the users desktop path
                string desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string path = desctopPath + "\\FileStructure.txt"; // Specify the output file's name
                int itteration = 0; // In case the file exists, what file version number should we append to the end
                                    // of the file name
                while (File.Exists(path)) // We keep looping for as long as the filename we specified exists
                {
                    itteration++;
                    // Update the path to use the new file name with the iterated file name
                    path = desctopPath + "\\FileStructure(" + itteration + ").txt";
                }
                File.Create(path).Dispose(); // Here we create the new file and dispose of it to stop accessing it
                TextWriter tw = new StreamWriter(path); // Load the file we created into the file writer
                TempTreeStructure = new List<string>(); // Override any previous data in 'TempTreeStructure' with a blank array
                //Output status messages
                Console.WriteLine();
                Console.WriteLine("Creating Tree Structure");
                DrawDirectoryTree(); // Initialize drawing the tree
                Console.WriteLine("Processing Tree Structure");
                foreach (string x in TempTreeStructure) // Loop through every string in the 'TempTreeStructure' array
                    tw.WriteLine(x); // Append to end of the text file with each line of the tree
                tw.Close(); // Stop accessing the test writer
                // Output information on the location of the file and how to return
                Console.WriteLine();
                Console.WriteLine("Created Tree Structure: {0}", path);
                Console.WriteLine();
                Console.WriteLine("Press Any Key To Return");
                ConsolePause(); // Pause the system
                Console.Clear(); // Clear the console
                MainMenu(); // Return to the main menu
            }
            else if (answer == "n" || answer == "no") // If the user answers no
            {
                Console.Clear(); // Clear the console
                MainMenu(); // Return to the main menu
            }
            else // If the user fails to enter yes or no
            {
                Console.Clear(); // Clear console
                DirectoryTree(); // Return to start of function
            }
        }

        /// <summary>
        ///     Here this function is used as a part of the inbuilt file soring in C#.It uses the global variable 'SortType' to
        ///     define what form of sorting will be applied to files when they are displayed to the screen.
        /// </summary>
        /// <param name="item">Current file that is being processed through the filter</param>
        /// <returns>Returns the data that needs to be sorted</returns>
        static Object SortByDefinition(FileInfo item)
        {
            switch (SortType) // Filter based on sort in 'SortType'
            {
                case 1: // Sort by file name
                    return item.Name;
                case 2: // Sort by Size
                    return item.Length;
                case 3: // Sort by Creation Time
                    return item.CreationTime;
                case 4: // Sort by Last Accessed
                    return item.LastAccessTime;
                case 5: // Sort by Extension
                    return item.Extension;
                default: // If out of range somehow, order by name
                    return item.Name;
            }
        }


        /// <summary>
        ///     Used to sort an array of files out either by ascending or descending order based on the global variable 'ASC'.
        /// </summary>
        /// <param name="files">Files you wish to be sorted</param>
        /// <returns>Returns an array of sorted files</returns>
        static FileInfo[] SortFileListing(FileInfo[] files)
        {
            if (SortType == 0) return files; // If SortType equals 0 (default) then return with nothing applied to 'files'
            if (ASC) // If we are sorting by ascending
            {
                // Here we order the files ascending using the inbuilt file function
                return files.OrderBy(item =>
                {
                    return SortByDefinition(item); // We use 'SortByDefinition' to define what we are sorting by
                }
                ).ToArray();
            }
            else // Else Descending
            {
                // Here we order the files ascending using the inbuilt file function
                return files.OrderByDescending(item =>
                {
                    return SortByDefinition(item); // We use 'SortByDefinition' to define what we are sorting by
                }
                ).ToArray();
            }
        }

        /// <summary>
        ///     Used to display the file listing menu. It takes a array of files that you wish to display and gives the user
        ///     the ability to get more detailed information about the files and execute '.exe' files
        /// </summary>
        /// <param name="files">Pass a array of files you wish to be outputted to the console</param>
        static void FileListingMenu(FileInfo[] files)
        {
            files = SortFileListing(files); // Sort the files based on pre-defined conditions
            bool notQuit = true; // Stays true until the user wishes to return to the main menu
            while (notQuit) // Loop whitest the user does not want to return to the main menu
            {
                // Output what directory we are currently in
                Console.WriteLine("Files in {0}", CurrentDirectory);
                Console.WriteLine();
                for (int i = 0; i < files.Length; i++) // Loop through all files in the directory
                {
                    FileInfo file = files[i]; // Get current file
                    double dynamicFileLength = file.Length; // Get current files size
                    int currentSizeSuffix = 0; // Define the index for the current file size suffix we should use
                    while (dynamicFileLength >= 1024 && // Loop through for every time 'dynamicFileLength' is larger then 1024
                        currentSizeSuffix < FileSizeSuffixs.Length - 1) // and whitest 'currentSizeSuffix' is less then 'FileSizeSuffixs.Length'
                    {
                        currentSizeSuffix++;
                        dynamicFileLength = dynamicFileLength / 1024; // Divide the variable 'dynamicFileLength' by 1024
                    }
                    OutputMessageWithParameters( // Output message
                        FileListingMessage, // Output the message stored in 'FileListingMessage'
                        i + 1, // File number that will be displayed on screen
                        file.Name, // Files current name
                        file.Length, // Files length in bytes
                        dynamicFileLength.ToString("0.00") + " " + FileSizeSuffixs[currentSizeSuffix] // Files size formated, EG (1.1GB)
                        );
                }
                // Output input instructions to the user
                Console.WriteLine();
                Console.WriteLine("0. Return to menu");
                Console.WriteLine();
                if (files.Length > 0) // If the file length is larger then 0 then we need to show the user how to access files 
                {
                    if (files.Length == 1) // If there is only one file then we don't need to show strings formated like (1-30) only (1)
                    {
                        // Output Information how to access the files information
                        Console.WriteLine("Enter [1] for more file information or [0] to return to the menu.");
                    }
                    else
                    {
                        // Output Information how to access the files information
                        Console.WriteLine("Enter [1-{0}] for more file information or [0] to return to the menu.", files.Length);
                    }
                }
                int selectedIndex = GetIntegerFromConsole(0, files.Length); // Get input from the user in the range from 0 to the files length
                if (selectedIndex == 0) // check to see if the user types 0, if so, let them return tot he main menu
                {
                    notQuit = false;
                }
                else
                {
                    FileInfo selectedFile = files[selectedIndex - 1]; // Get the file at the selected index
                    Console.Clear(); // Clear the console
                    DisplayFileInformation(selectedFile); // Show information based on the file 
                    Console.WriteLine();
                    if (selectedFile.Extension == ".exe") // Check to see if the selected file is an .exe file, if so, enable executing the file
                    {
                        // Output controls to the user on how to execute the file
                        Console.WriteLine("Enter [0] To Return or [1] to execute");
                        selectedIndex = GetIntegerFromConsole(0, 1); // Get a value from the user in the range of 0-1
                        if (selectedIndex == 1) // If the user selected 1 then execute the file
                        {
                            System.Diagnostics.Process.Start(selectedFile.FullName);
                        }
                        Console.Clear(); // Clear the console
                    }
                    else
                    {
                        // Output a message to let the user return to the file listing
                        Console.WriteLine("Press Enter To Return");
                        ConsolePause();
                    }
                    Console.Clear(); // Clear the console
                }
            }
            Console.Clear(); // Clear the console
            MainMenu(); // Return the user to the main menu
        }

        /// <summary>
        ///     Gives detailed statistics regarding the folder defined in the variable 'CurrentDirectory'.
        /// </summary>
        static void FolderStatistics()
        {
            DirectoryInfo folderInfo = new DirectoryInfo(CurrentDirectory); // Gets information on the current directory
            FileInfo[] files = folderInfo.GetFiles(); // Gets all files in the current directory and stores it in a array
            FileInfo largestFile = null; // Initialize a variable that will store the largest file from the directory in
            long totalFileSize = 0L; // Initialize a variable that will store the total size of every file in the folder
            foreach (FileInfo file in files) // Loop through every file in the directory
            {
                if (largestFile == null || // Check to see if the largest file is null 
                    largestFile.Length < file.Length) // or check to see if the current largest file is smaller then the current file
                    largestFile = file; // Set largest file to be the current file being processed
                totalFileSize += file.Length; // Add the current files size to 'totalFileSize'
            }
            long averageFileSize = totalFileSize / files.Length; // Work out the average file length
            //Output the folders information
            Console.WriteLine("Files in {0}", CurrentDirectory);
            Console.WriteLine();
            Console.WriteLine("Total files: {0}", files.Length);
            Console.WriteLine("Total size of all files: {0} bytes", totalFileSize);
            if (largestFile != null) // Check to see if the file is not null, if this is the case then output the largest file
                Console.WriteLine("Largest file: {0}, {1} bytes", largestFile.Name, largestFile.Length);
            Console.WriteLine("Average file length: {0} bytes", averageFileSize);
            Console.WriteLine();
            Console.WriteLine("Press Any Key To Return");
            ConsolePause(); // Pause the application
            Console.Clear(); // Clear the console
            MainMenu(); // Return to the main menu
        }

        /// <summary>
        ///     Displays all sub directory's in current directory 'CurrentDirectory' and gives the user the ability to
        ///     navigate to the required directory.
        /// </summary>
        static void ChangeDirectory()
        {
            bool changeingDirectory = true; // Remains true whilst we are changing the directory
            string newDirectory = CurrentDirectory; // Create a temporary variable to store the path thats changing, into.
            while (changeingDirectory) // Loop whitest the user wants to change the directory
            {
                // Output information regarding the current directory and the directory we are looking at
                Console.WriteLine("Current Directory {0}", CurrentDirectory);
                Console.WriteLine("New Directory {0}", newDirectory);
                Console.WriteLine();
                // Here we get the directory's in the directory stored in 'newDirectory'
                string[] directorys = Directory.GetDirectories(newDirectory);
                // Here we get information regarding the current directory
                DirectoryInfo folderInfo = new DirectoryInfo(newDirectory);
                for (int i = 0; i < directorys.Length; i++) // Loop through all the directory's
                {
                    string dir = directorys[i]; // Get the current focused on, directory
                    Console.WriteLine("{0}: " + dir, i + 1); // Output the directory path to the screen
                }
                Console.WriteLine();
                if (folderInfo.Parent != null) // We make sure the parent directory is not null. Would only happen in the root (C:\\)
                    Console.WriteLine("Enter [{0}] to go up a level", directorys.Length + 1); // Output information to navigate up one level
                Console.WriteLine("Enter [0] to save and exit"); // Output information on how to exit and return to the Main Menu
                if (directorys.Length > 0) // Check to see if there is one or more directory
                    Console.WriteLine("[1-{0}] to enter sub-directory", directorys.Length); // Output information on how to navigate the directory's
                int maxBounds = directorys.Length; // Here we define a variable that contains the max integer that a user should enter
                if (folderInfo.Parent != null) // Check to see if the parent is not null
                    maxBounds++; // Add one to max bounds because we can go up a level
                int selectedIndex = GetIntegerFromConsole(0, maxBounds); // Pulls integer value from the console equal too or over the minimum value
                if (selectedIndex == 0) // if the user inputed 0 then we return
                {
                    changeingDirectory = false; // Break from loop
                    CurrentDirectory = newDirectory; // Update the 'CurrentDirectory' with the new directory
                }
                else if (selectedIndex == directorys.Length + 1) // check to see if the user wants to go up a level
                {
                    if (folderInfo.Parent != null) // Make sure that the current directory in 'newDirectory' has a parent folder
                        newDirectory = folderInfo.Parent.FullName; // Update the 'newDirectory' to the parents path
                }
                else
                {
                    newDirectory = directorys[selectedIndex - 1]; // Update the 'newDirectory' to the defined sub folder
                }
                Console.Clear(); // Clear the console
            }
            MainMenu(); // return to the main menu
        }

        /// <summary>
        ///     Display all sort options to the user and gives them the ability to change sorting option's and order from
        ///     ascending to descending.
        /// </summary>
        static void ChangeSort()
        {
            bool changingSort = true; // Remains true whilst we are updating the sort
            while (changingSort) // Loops whitest the user is updating the sort
            {
                //Output information regarding the current sort
                Console.WriteLine("Current Sort");
                Console.WriteLine();
                Console.WriteLine("Sort By: {0}", SortDefinitions[SortType]); // Outputs the current sort based on SortType as the index
                if (SortType > 0) // Check to see if SortType is not the default sorting method
                    Console.WriteLine("Sort Ascending: {0}", ASC); // Output the current order that it will be sorted by
                Console.WriteLine();
                for (int i = 0; i < SortDefinitions.Length; i++) // Loop through the sort definitions
                {
                    Console.WriteLine("{0}. {1}", i + 1, SortDefinitions[i]); // Output the current sort definition
                }
                // Output information on how to exit and how to change sorting method
                Console.WriteLine();
                Console.WriteLine("Enter [0] to exit");
                Console.WriteLine("[1-{0}] to change the sorting method", SortDefinitions.Length);
                if (SortType > 0) // Check to see if SortType is not the default sorting method
                    Console.WriteLine("[{0}] to toggle sorting order", SortDefinitions.Length + 1); // Output valid use input options
                // Get the users input regarding which sorting method they wish to use 
                int selectedIndex = GetIntegerFromConsole(0, SortType > 0 ? SortDefinitions.Length + 1 : SortDefinitions.Length);
                if (selectedIndex == 0) // If the user enters 0 then return to the Main Menu
                {
                    changingSort = false; // Break out of the loop
                }
                else if (SortType > 0 && // Make sure the SortType is not default (0)
                    SortDefinitions.Length + 1 == selectedIndex) // and make sure what the user inputed equals SortDefinitions.Length + 1
                {
                    ASC = !ASC; // Flip the value of ASC
                }
                else
                {
                    SortType = selectedIndex - 1; // Change the Sort Type to be the value the user inputed - 1
                }
                Console.Clear(); // Clear the console
            }
            MainMenu(); // Return to the Main Menu
        }

        // Main function
        static void Main(string[] args)
        {
            initilization(); // Initialize all global variables
            MainMenu(); // Load up the main menu
        }
    }
}
