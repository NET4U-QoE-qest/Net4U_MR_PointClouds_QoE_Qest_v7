using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
 
public static class RuntimePLYLoader
{
    public static Mesh LoadMeshFromPLY(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[RuntimePLYLoader] File not found: {filePath}");
            return null;
        }

        try
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BinaryReader reader = new BinaryReader(fs);

            // read header
            string line;
            int vertexCount = 0;
            long dataStart = 0;

            while (true)
            {
                line = ReadLine(reader);
                if (line == null)
                {
                    Debug.LogError("[RuntimePLYLoader] Unexpected end of header");
                    return null;
                }

                if (line.StartsWith("element vertex"))
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length >= 3)
                        vertexCount = int.Parse(parts[2]);
                }
                else if (line.StartsWith("end_header"))
                {
                    dataStart = fs.Position;
                    break;
                }
            }

            if (vertexCount == 0)
            {
                Debug.LogWarning($"[RuntimePLYLoader] No vertex count found in header of {filePath}");
                return null;
            }

            
            fs.Seek(dataStart, SeekOrigin.Begin);

            var vertices = new List<Vector3>(vertexCount);
            var colors = new List<Color32>(vertexCount);

            for (int i = 0; i < vertexCount; i++)
            {
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                double z = reader.ReadDouble();

                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte r = reader.ReadByte();

                vertices.Add(new Vector3((float)x, (float)y, (float)z));
                colors.Add(new Color32(r, g, b, 255));
            }

            Mesh mesh = new Mesh
            {
                name = Path.GetFileNameWithoutExtension(filePath),
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(CreateSequentialIndices(vertices.Count), MeshTopology.Points, 0);
            mesh.SetColors(colors);
            mesh.RecalculateBounds();

            Debug.Log($"[RuntimePLYLoader]  Loaded {vertices.Count} points from: {filePath}");
            return mesh;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[RuntimePLYLoader]  Failed to load {filePath}: {ex.Message}");
            return null;
        }
    }

    private static string ReadLine(BinaryReader reader)
    {
        List<byte> bytes = new List<byte>();
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            byte b = reader.ReadByte();
            if (b == '\n') break;
            if (b != '\r') bytes.Add(b);
        }
        return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static int[] CreateSequentialIndices(int count)
    {
        int[] indices = new int[count];
        for (int i = 0; i < count; i++) indices[i] = i;
        return indices;
    }
}