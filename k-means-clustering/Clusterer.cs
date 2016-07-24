using System;

namespace k_means_clustering
{

    /// <summary>
    /// k-means clustering algorithm
    /// </summary>
    public class Clusterer
    {

        private int _numClusters;       //number of clusters
        private int[] _clustering;      // index = a tuple and value = cluster ID
        private double[][] _centroids;  //the centroid (mean (vector)) for each cluster
        private static Random _rnd;     //random for initialization

        #region Public Methods
        /// <summary>
        /// Constructor, allocates the rows of data member matrix _centroids, but not columns yet b/c those can't
        /// be known until the data to be clustered is presented.
        /// We can't allocate _clustering yet either, b/c the # of items is not known yet
        /// TODO: Have the value for random be passed in as a param to the constuctor b/c diff random values yield "significantly" diff results.
        /// TODO: Alternative design would be to pass the rawData in here so we can allocate everything pp. 23
        /// </summary>
        /// <param name="numClusters">The number of clusters, or 'k'</param>
        public Clusterer(int numClusters)
        {
            //HACK: No error checking, numClusters must be <2
            this._numClusters = numClusters;
            this._centroids = new double[numClusters][];
            _rnd = new Random(0);
        }

        public int[] Cluster(double[][] data)
        {
            //determine the number of data items to be clusters
            int numTuples = data.Length;
            //determine the number of values for each data item
            int numValues = data[0].Length;
            //allocate array _clustering now the we know the data size
            this._clustering = new int[numTuples];

            //allocate the columns of data member matrix _centroids
            for(int k = 0; k < _numClusters; ++k)
            {
                //give each centroid the memory for the number of values of the data elements (in this case 2)
                this._centroids[k] = new double[numValues];
            }

            //initialize the clustering with random assignments by calling helper mehtod InitRandom
            InitRandom(data);
            Console.WriteLine("\nInitial Random Clustering has yielded:");
            for (int i = 0; i < _clustering.Length; ++i)
                Console.Write(_clustering[i] + " ");
            Console.WriteLine("\n");

            //the "heart" of the method where we update centroids and clustering
            bool changed = true;
            int maxCount = numTuples * 10; //sanity check
            int ct = 0;
            while (changed = true && ct <= maxCount)
            {
                ++ct;
                //compute the centroid for each cluster
                UpdateCentroids(data);
                //reassigns each data item to the cluster that is associated with the closest centroid, returns false if no data items change clusters (we're done at that point)
                changed = UpdateClustering(data);
            }
            //copy the results of clustering and return them, (why not return this._clustering directly?
            int[] result = new int[numTuples];
            Array.Copy(this._clustering, result, this._clustering.Length);
            if (ct == maxCount)
                Console.WriteLine("ERROR: Max count was reached! Something's wrong!");
            return result;
        }
        #endregion

        #region Private Methods
        private void InitRandom(double[][] data) //, int maxAttempts)
        {
            int numTuples = data.Length;
            int clusterID = 0;
            //assign initial values by stepping through them
            for (int i = 0; i < numTuples; ++i)
            {
                _clustering[i] = clusterID++;
                if (clusterID == _numClusters)
                    clusterID = 0;
            }

            //Implement Fisher-Yates shuffle on those values
            for (int i = 0; i < numTuples; ++i)
            {
                int r = _rnd.Next(i, _clustering.Length); //pick a cell
                int tmp = _clustering[r]; //get that cells value
                _clustering[r] = _clustering[i];
                _clustering[i] = tmp;
            }
        }

        /// <summary>
        /// Update each of the centroids so that they have associated data tuples
        /// The average is obtained but im not quite sure for what yet or exactly how
        /// </summary>
        /// <param name="data"></param>
        private void UpdateCentroids(double[][] data)
        {
            /*start by computing the current number of data tuples
             *This is needed to compute the average of each centroid component
             * TODO: I could make clusterCounts a class member to access it if so desired
            */
            int[] clusterCounts = new int[_numClusters];
            for(int i = 0; i < data.Length; ++i)
            {
                //get the cluster id
                int clusterID = _clustering[i];
                //increase the cluster count for a particular id
                ++clusterCounts[clusterID];
            }

            //zero out the current cells in the this.centroid matrix
            //adding 0.0 b/c there are currently no values, null
            for (int k = 0; k < _centroids.Length; ++k)
                for (int j = 0; j < _centroids[k].Length; ++j)
                    _centroids[k][j] = 0.0;

            //Accumulate the sums, add the data tuples to the centroids at a given clusterID
            for (int i = 0; i < data.Length; ++i)
            {
                int clusterID = _clustering[i];
                for (int j = 0; j < data[i].Length; ++j)
                    _centroids[clusterID][j] += data[i][j]; //accumulate sum
            }

            //divide the accumulated sums by the appropriate cluster count
            //this gets us the average
            for (int k = 0; k < _centroids.Length; ++k)
                for (int j = 0; j < _centroids[k].Length; ++j)
                    _centroids[k][j] /= clusterCounts[k]; //danger will robinson
                    //TODO: fatal divide / 0 error can occur if a cluster has no data tuples (more below)
                    //This can happen if at intial spawn a centroid has a tuple, but then during update
                    //it moves to a diff centroid, one *could* be left with 0. No immediate recommendations
                    //for a solution. pp. 27
        }

        /// <summary>
        /// Method to "(re)assign each tuple to a cluster (closest centroid)
        /// returns false if no tuple assignment change (meaning we're done) OR
        /// if the reassignment would result in a clustering where
        /// one or more clusters have not tuple (data points)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool UpdateClustering(double[][] data)
        {
            //has any tuple changed clusters?
            bool changed = false;
            //the new (proposed) clustering array allocated based on the originals length
            int[] newClustering = new int[_clustering.Length];
            //copy the arrays
            Array.Copy(_clustering, newClustering, _clustering.Length);
            //hold the distance from a given data tuple to each centroid
            //e.g. {4.0, 1.5, 2.8} given a tuple where 4 is dist to cluster 0, 1.5 is dist to cluster 1, and 2.8 is dist to cluster 2
            double[] distances = new double[_numClusters]; // "from tuple to centroids"

            //For each tuple of data
            for(int i = 0; i< data.Length; ++i)
            {
                //for each 'k' cluster, compute the distance of data at i to the centroid 'k'
                for (int k = 0; k < _numClusters; ++k)
                    distances[k] = Distance(data[i], _centroids[k]);

                //"find closest centroid"
                int newClusterID = MinIndex(distances);

                if (newClusterID != newClustering[i])
                {
                    //indicate that a change has occured
                    changed = true;
                    //Update the value of new clustering to be the new one we've found
                    newClustering[i] = newClusterID;
                }
            }

            //exit early if there is no change
            if (changed == false)
                return false;

            //another exit early check, if a cluster has no data tuple associated with it
            //create an array clusterCounts the size of the number of clusters
            int[] clusterCounts = new int[_numClusters];
            //iterate through our data tuples
            for(int i = 0; i < data.Length; ++i)
            {
                //get the cluster id from the proposed clustering
                int clusterID = newClustering[i];
                //add one to the cluster id
                ++clusterCounts[clusterID];
            }

            //if a cluster was found with no tupes, return false
            for (int k = 0; k < _numClusters; ++k)
                if (clusterCounts[k] == 0)
                    Console.WriteLine("ERROR: Centroid with 0 tuples associated found. Shitting!");
                    return false; //cluster with 0 tuples found

            //TODO: Implement a possible workaround where a random tuple is pulled from a centroid with 2 or more and assigned to the centroid with 0, i dunno if i like this

            //All is well, copy the new clustering array to the current
            Array.Copy(newClustering, this._clustering, newClustering.Length);
            //"Good clustering and at least one change"
            return true;

        }

        /// <summary>
        /// Compute the Euclidean distance between a tuple to a centroid
        /// TODO: Research alternative distance calculation methods, such as 'cosine distance'
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private static double Distance(double[] tuple, double[] centroid)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
            {
                sumSquaredDiffs += (tuple[j] - centroid[j]) * (tuple[j] - centroid[j]);
            }
            return Math.Sqrt(sumSquaredDiffs);
        }

        /// <summary>
        /// Identify the cluster ID with the smallest distance
        /// Locates the index of the smallest value in an array
        /// For k-means, this index is equivalent to the cluster ID of the closest centroid
        /// TODO: Further understand and improve this method, its suggested an alternative could be to remove
        /// 'static' so that distances.Length can be replaced with this._numClusters
        /// </summary>
        /// <param name="distances"></param>
        /// <returns></returns>
        private static int MinIndex(double[] distances)
        {
            //init local var as 0
            int indexOfMin = 0;
            //init the smallest distance as the first distance
            double smallDist = distances[0];
            
            //iterate through each of the distances
            for (int k = 1; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }
        #endregion

    } //class
} //ns

