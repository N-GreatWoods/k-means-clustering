using System;

namespace k_means_clustering
{
    /// <summary>
    /// Program to perform K Means Clustering
    /// </summary>
    class KMeansClusterProgram
    {
        /// <summary>
        /// Driver method 'main'
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("\nDankNet K-Means Clustering Start\n");

            string dataFile = "F:\\Projects\\c_sharp\\InputData\\HeightWeight.txt";
            double[][] rawData = LoadData(dataFile, 10, 2, ',');

            //double[][] rawData = new double[10][];
            //rawData[0] = new double[] { 73.0, 72.6 };
            //rawData[1] = new double[] { 61.0, 54.4 };
            //rawData[2] = new double[] { 67.0, 99.9 };
            //rawData[3] = new double[] { 68.0, 97.3 };
            //rawData[4] = new double[] { 62.0, 59.0 };
            //rawData[5] = new double[] { 75.0, 81.6 };
            //rawData[6] = new double[] { 74.0, 77.1 };
            //rawData[7] = new double[] { 66.0, 97.3 };
            //rawData[8] = new double[] { 68.0, 93.3 };
            //rawData[9] = new double[] { 61.0, 59.0 };

            /* SHOWING STARTING DATA */
            Console.WriteLine("Raw unclustered data:\n");
            Console.WriteLine(" ID Height (in.) Weight (kg.)");
            Console.WriteLine("---------------------------------");
            ShowData(rawData, 1, true, false);

            /* INIT CLUSTERING */
            int numClusters = 3;
            Console.WriteLine("\nSetting numClusters to " + numClusters);

            Console.WriteLine("\nStarting clustering using k-means algoritm");
            //init Clusterer with number of clusters
            Clusterer c = new Clusterer(numClusters);
            //give the matrix of data to the clusterer
            //returns int array where array index value is the index of a data item, and the array cell value is a cluster ID
            //the array indices (0, 1, 2, .. 9) represent indices of the data items
            //the cell values {2, 0, 1 .. 2} represent the cluster ids
            //Could be done many diff ways, and I quote:
            //"For example, you could use an array of List objects, where each List
            //collection holds the indices of data items that belong to the same
            //cluster." pp 21.
            int[] clustering = c.Cluster(rawData);
            Console.WriteLine("Clustering Complete!\n");

            /* SHOWING FINAL DATA */
            Console.WriteLine("Final clustering in internal form:\n");
            ShowVector(clustering, true);

            Console.WriteLine("Raw data by cluster:\n");
            ShowClustered(rawData, clustering, numClusters, 1);

            Console.WriteLine("\nEnd k-means clustering demo\n");
            Console.ReadLine();

        }

        /// <summary>
        /// Show the starting data
        /// TODO: Possibly add a param of string[] that containers headers?
        /// </summary>
        /// <param name="data">matrix of data to display</param>
        /// <param name="decimals">the number of decimals to display</param>
        /// <param name="indices">show indices bool</param>
        /// <param name="newLine">show newline bool</param>
        static void ShowData(double[][] data, int decimals, bool indices, bool newLine)
        {
            //iterate through each of the indices of the matrix
            for (int i = 0; i < data.Length; ++i)
            {
                //if we are displaying indices, display them now
                if (indices == true)
                    Console.WriteLine(i.ToString().PadLeft(3) + " ");
                //for each indices, iterate through the data within
                for(int j = 0; j < data[i].Length; ++j)
                {
                    //set v to the value of data at i index and j value
                    double v = data[i][j];
                    //the 'F' is for Fixed-point with decimals as a "precision specifier"
                    Console.WriteLine(v.ToString("F" + decimals) + "   ");
                } // j
                //add a newline by default after each element
                Console.WriteLine("");
            } // i
            //if we are to show a newline, add one
            if (newLine == true)
                Console.WriteLine("");
        }

        /// <summary>
        /// Helper method to display the internal clustering representation
        /// TODO: Make a ToString method for the class to show this data
        /// </summary>
        /// <param name="vector">The resultant data as an int array</param>
        /// <param name="newLine">show newline bool</param>
        static void ShowVector(int[] vector, bool newLine)
        {

            for (int i = 0; i < vector.Length; ++i)
            {
                Console.Write(vector[i] + " ");
            }
            if (newLine == true)
                Console.WriteLine("\n");
        }

        /// <summary>
        /// Helper method to dispaly the source data after it has been clustered, grouped by cluster
        /// TODO: And I quote:
        /// "Method ShowClustered makes multiple passes through the data set that
		/// has been clustered. A more efficient, but significantly more
		/// complicated alternative, is to define a local data structure, such as
		/// an array of List objects, and then make a first, single pass through
		/// the data, storing the clusterIDs associated with each data item. Then
		/// a second, single pass through the data structure could print the data
		/// in clustered form."
        /// </summary>
        /// <param name="data">matrix of data to display</param>
        /// <param name="clustering"></param>
        /// <param name="numClusters"></param>
        /// <param name="decimals"></param>
        static void ShowClustered(double[][] data, int[] clustering, int numClusters, int decimals)
        {
            //Start by iterating by 'k' the chosen number of clusters used by the algorithm
            for (int k = 0; k < numClusters; ++k)
            {
                Console.WriteLine("==========================");
                //Iterate through each index in the main data matrix
                for (int i = 0; i < data.Length; ++i)
                {
                    //assign the clusterID to the data element associated with the clustering element at data element i
                    //tl;dr get the ID of the data element post clustering
                    int clusterID = clustering[i];
                    //if the clusterID is not for this cluster, 'pass'
                    if (clusterID != k) continue;
                    Console.Write(i.ToString().PadLeft(3) + " ");
                    for (int j = 0; j < data[i].Length; ++j)
                    {
                        //get the actual data value
                        double v = data[i][j];
                        //the 'F' is for Fixed-point with decimals as a "precision specifier"
                        Console.Write(v.ToString("F" + decimals) + " ");
                    } // j
                    Console.WriteLine("");
                } // i
                Console.WriteLine("==========================");
            } // k - nice i like this notation!
        }

        /// <summary>
        /// Load data from a file
        /// TODO: Update to dynamically determine the number of rows
        /// </summary>
        /// <param name="dataFile"></param>
        /// <param name="numRows"></param>
        /// <param name="numCols"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        static double[][] LoadData(string dataFile, int numRows, int numCols, char delim)
        {

            /*
             * Dynamic row/column reading pseudo code
            numRows := 0
            open file
            while not EOF
            numRows := numRows + 1
            end loop
            close file
            allocate result array with numRows
            open file
            while not EOF
            read and parse line with numCols
            allocate curr row of array with numCols
            store line
            end loop
            close file
            return result matrix
            */

            //open the file specified
            System.IO.FileStream ifs = new System.IO.FileStream(dataFile, System.IO.FileMode.Open);
            //Read the file as a stream
            System.IO.StreamReader sr = new System.IO.StreamReader(ifs);
            //init local var line as empty
            string line = "";
            //set init tokens to null (where tokens are each element of a line)
            string[] tokens = null;
            //init local var i
            int i = 0;
            //init local var result with known rows
            double[][] result = new double[numRows][];
            //while the lines being read are not null
            while((line = sr.ReadLine()) != null)
            {
                //Start storing the results where this index of result is the values of the line
                result[i] = new double[numCols];
                tokens = line.Split(delim);
                for (int j = 0; j < numCols; ++j)
                {
                    //for the result line, set j to the values
                    result[i][j] = double.Parse(tokens[j]);
                }
                //increment the line number
                ++i;
            }
            //Close the stream reader
            sr.Close();
            //Close the FS
            ifs.Close();
            return result;
        }
    } // class
} // ns
