using System;

namespace k_means_clustering
{

    /// <summary>
    /// k-means clustering algorithm
    /// </summary>
    public class Clusterer
    {

        private int _numClusters;       //number of clusters

        private Random _rnd;            //random for initialization

        public Clusterer(int numClusters)
        {
            this._numClusters = numClusters;
            this._rnd = new Random(0); //arbitrary seed
        }

        public int[] Cluster(double[][] data)
        {
            int[] result = new int[4];
            return result;
        }
    }


}

