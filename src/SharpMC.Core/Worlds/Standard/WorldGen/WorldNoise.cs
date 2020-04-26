using System;
using SharpNoise.Modules;

namespace SharpMC.Core.Worlds.Standard.WorldGen
{
    class WorldNoise
    {
        private Perlin cavePerlin;
        private Perlin biomePerlin;
        private int biome_size;
        private float cave_fill;
        private float cave_height;
        private float cave_stretch;
        const float WORLD_HORZ_STRETCH_FACTOR = 1 / 6400.0f; //0.000008925f;
        const float WORLD_VERT_STRETCH_FACTOR = 1.0f;//0.018625f;

        WorldGenerationSettings WorldSettings;
        WorldGenerator WorldGen;
        Module WorldGenModule;

        private static readonly Lazy<WorldNoise> lazy = new Lazy<WorldNoise>(() => new WorldNoise());

        public static WorldNoise Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private WorldNoise()
        {
            // Todo: get these settings from WorldGenerationSettings
            biome_size = 2 * 100;
            cave_fill = (20 / 100.0f * 0.70f) + 0.30f;
            cave_height = 50 / 100.0f * 0.25f;
            cave_stretch = 80 / 1000.0f * 0.8f;

            WorldSettings = new WorldGenerationSettings();
            WorldGen = new WorldGenerator(WorldSettings);
            WorldGenModule = WorldGen.CreateModule();

            biomePerlin = new Perlin
            {
                Seed = 1,
                Frequency = 3
            };

            cavePerlin = new Perlin
            {
                Seed = 1,
                Lacunarity = 0,
                Quality = SharpNoise.NoiseQuality.Fast,
                OctaveCount = 1
            };
        }

        public void SetWorldGeneratorSettings(WorldGenerationSettings settings)
        {
            WorldGenModule = new WorldGenerator(settings).CreateModule();
            biomePerlin.Seed = settings.Seed;
            cavePerlin.Seed = settings.Seed;
        }

        public double Biome(int bx, int bz)
        {
            return WorldGen.RiverPositions.GetValue(bx, 0, bz);
        }

        //Function that inputs the position and spits out a float value based on the perlin noise
        public double Terrain(float x, float z)
        {
            double terrainOffset = 0;
            try
            {
                terrainOffset = WorldGenModule.GetValue(x * WORLD_HORZ_STRETCH_FACTOR, 0, z * WORLD_HORZ_STRETCH_FACTOR)+20;
            }
            catch(Exception e)
            {

            }
            //return terrainOffset < 60 ? 60 : terrainOffset;
            return terrainOffset;

        }

        public bool Cave(float x, float y, float z)
        {
            y *= cave_height;
            x *= cave_stretch;
            z *= cave_stretch;
            double val = cavePerlin.GetValue(x, y, z);
            return !(val < cave_fill && val > 0);
        }
    }
}
