using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class STTaskBase
{
    public List<STSequence> sequences = new List<STSequence>();
    public int currentSequenceIndex = -1;

    protected string qualityPrefix = "r";
    protected string distancePrefix = "d";

    public string currentSequenceString = "";

    private Mesh[] firstMeshes;
    private Mesh[] secondMeshes;

    AnimatePointCloudBase animateComp;

    public STTaskBase(List<STSequence> sequences)
    {
        this.sequences = sequences;
    }

    protected void ActivateCurrentPointCloud()
    {
        animateComp.StartAnimation();
        Debug.Log("Activating point cloud object.");
    }

    protected string MakeSequenceString(string currentObjectString, string currentQualityString, string currentDistanceString)
    {
        return currentObjectString + "_" + currentQualityString + "_" + currentDistanceString;
    }

    public void SetupNextSequence()
    {
        currentSequenceIndex += 1;

        if (currentSequenceIndex == sequences.Count)
        {
            OnTaskEnded();
            return;
        }

        STSequence currSequence = sequences[currentSequenceIndex];
        string pcName = currSequence.ObjectType.ToString(); // Use enum name as folder name
        PointCloudObject currPcObject = PointCloudsLoader.Instance.pcObjects.FirstOrDefault(p => p.pcName == pcName);

        if (currPcObject == null)
        {
            Debug.LogError($"PointCloudObject not found: {pcName}");
            return;
        }

        if (currSequence.RepresentationType == PointCloudRepresentation.Mesh)
        {
            firstMeshes = currPcObject.meshes[currSequence.FirstQuality];
            firstMeshes = firstMeshes.Take(firstMeshes.Length / 2).ToArray();

            secondMeshes = currPcObject.meshes[currSequence.SecondQuality];
            secondMeshes = secondMeshes.Skip(secondMeshes.Length / 2).ToArray();

            Material[] firstMaterials = currPcObject.meshMaterials[currSequence.FirstQuality];
            firstMaterials = firstMaterials.Take(firstMaterials.Length / 2).ToArray();

            Material[] secondMaterials = currPcObject.meshMaterials[currSequence.SecondQuality];
            secondMaterials = secondMaterials.Skip(secondMaterials.Length / 2).ToArray();

            animateComp.meshMaterials = firstMaterials.Concat(secondMaterials).ToArray();
            animateComp.SetIsMesh(true);
        }
        else
        {
            firstMeshes = currPcObject.frames.Take(currPcObject.frames.Count / 2).ToArray();
            secondMeshes = currPcObject.frames.Skip(currPcObject.frames.Count / 2).ToArray();

            animateComp.SetIsMesh(false);

            var rend = animateComp.GetComponent<MeshRenderer>();
            if (rend != null)
                rend.materials = new Material[] { STManager.Instance.GetMaterialFromRepresentation(currSequence.RepresentationType) };
        }

        animateComp.CurrentMeshes = firstMeshes.Concat(secondMeshes).ToArray();
        STManager.Instance.SetCurrentPCDistance(currSequence.Distance);

        if (currentSequenceIndex < sequences.Count)
        {
            string firstQuality = currSequence.FirstQuality.ToString();
            string secondQuality = currSequence.SecondQuality.ToString();

            string qualityString = firstQuality + "_" + qualityPrefix + secondQuality;
            string distanceString = ((int)(currSequence.Distance * 100f)).ToString();

            string outTextFormatted = MakeSequenceString(
                                pcName,
                                qualityPrefix + qualityString,
                                distancePrefix + distanceString);

            currentSequenceString = outTextFormatted;
            STManager.Instance.SetDisplayString(outTextFormatted);
            Debug.Log(outTextFormatted);
        }

        ActivateCurrentPointCloud();
    }

    public void SetupTask()
    {
        animateComp = STSecondaryManager.Instance.currentGameObject.GetComponent<AnimatePointCloudBase>();
        currentSequenceIndex = -1;
        SetupNextSequence();
    }

    private void OnTaskEnded()
    {
        STManager.Instance.OnFullTaskEnded();
        Debug.Log("Task ended.");
    }

    void SetSequences(List<STSequence> sequences)
    {
        this.sequences = sequences;
    }
}
