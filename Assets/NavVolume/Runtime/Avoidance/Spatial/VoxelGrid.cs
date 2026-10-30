using Unity.Mathematics;

namespace NavVolume.Runtime.Avoidance
{
    /// <summary>
    /// Burst-readable view over one <see cref="NavVolumeSpace"/>'s finest-resolution occupancy.
    /// </summary>
    internal struct VoxelGrid
    {
        /// <summary>
        /// First node of this grid inside the shared morton/mask pools.
        /// </summary>
        public int NodeStart;

        /// <summary>
        /// Number of layer-0 nodes.
        /// </summary>
        public int NodeCount;

        /// <summary>
        /// Number of layer-0 nodes per axis.
        /// </summary>
        public int NodeGridDim;

        /// <summary>
        /// World-space minimum corner of the volume.
        /// </summary>
        public float3 Origin;

        public float VoxelSize;

        /// <summary>
        /// World size of a layer-0 node (4 voxels).
        /// </summary>
        public float NodeSize;

        /// <summary>
        /// Agent radius already included in this grid's occupied voxels when it was built.
        /// </summary>
        public float BakedAgentRadius;

        /// <summary>
        /// Returns the clearance still needed beyond the radius included in the bake.
        /// </summary>
        public readonly float GetAdditionalRadius(float agentRadius) =>
            math.max(0f, agentRadius - BakedAgentRadius);
    }
}
