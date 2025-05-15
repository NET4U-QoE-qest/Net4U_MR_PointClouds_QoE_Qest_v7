using System;
using System.IO;
using UnityEngine;

namespace Pcx
{
    public static class PLYImporter
    {
        public static Mesh ImportBinary(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[PLYImporter] File not found: {filePath}");
                return null;
            }

            try
            {
                using (var stream = File.OpenRead(filePath))
                using (var reader = new BinaryReader(stream))
                {
                    // Header parsing
                    string line;
                    int vertexCount = 0;
                    bool headerEnded = false;
                    bool hasColor = false;

                    while (!headerEnded && (line = ReadLine(reader)) != null)
                    {
                        if (line.StartsWith("element vertex"))
                        {
                            string[] parts = line.Split(' ');
                            vertexCount = int.Parse(parts[2]);
                        }
                        else if (line.StartsWith("property uchar red"))
                        {
                            hasColor = true;
                        }
                        else if (line.StartsWith("end_header"))
                        {
                            headerEnded = true;
                        }
                    }

                    if (vertexCount <= 0)
                    {
                        Debug.LogError("[PLYImporter] Invalid vertex count or missing header");
                        return null;
                    }

                    // Vertex reading
                    Vector3[] vertices = new Vector3[vertexCount];
                    Color[] colors = hasColor ? new Color[vertexCount] : null;
                    int[] indices = new int[vertexCount];

                    for (int i = 0; i < vertexCount; i++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        vertices[i] = new Vector3(x, y, z);

                        if (hasColor)
                        {
                            byte r = reader.ReadByte();
                            byte g = reader.ReadByte();
                            byte b = reader.ReadByte();
                            colors[i] = new Color32(r, g, b, 255);
                        }

                        indices[i] = i;
                    }

                    // Mesh
                    Mesh mesh = new Mesh();
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.vertices = vertices;
                    mesh.colors = colors;
                    mesh.SetIndices(indices, MeshTopology.Points, 0);
                    mesh.RecalculateBounds();

                    return mesh;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PLYImporter] Error during parsing: {ex.Message}");
                return null;
            }
        }

        private static string ReadLine(BinaryReader reader)
        {
            var line = new System.Text.StringBuilder();
            char c;
            while (reader.BaseStream.Position < reader.BaseStream.Length &&
                   (c = reader.ReadChar()) != '\n')
            {
                line.Append(c);
            }
            return line.ToString().Trim();
        }
    }
}
