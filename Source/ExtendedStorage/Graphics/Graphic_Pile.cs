using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ExtendedStorage.Graphics;

// Composite graphic that draws multiple child graphics layered at the same position
internal class Graphic_Pile : Graphic
{
    private readonly float _altitudeStep;
    private readonly List<Graphic> _layers;

    public Graphic_Pile(List<Graphic> layers, float altitudeStep = 0.0001f)
    {
        _layers = layers ?? new List<Graphic>();
        _altitudeStep = altitudeStep;
    }

    public override Material MatSingle => _layers.Count > 0 ? _layers[0].MatSingle : BaseContent.BadMat;

    public override bool ShouldDrawRotated => false;

    // Draw helper using ThingDef, delegates drawing to child graphics
    public void DrawFromDef(Vector3 loc, Rot4 rot, ThingDef thingDef)
    {
        if (_layers == null || _layers.Count == 0)
        {
            return;
        }

        for (var i = 0; i < _layers.Count; i++)
        {
            var g = _layers[i];
            var pos = new Vector3(loc.x, loc.y + (i * _altitudeStep), loc.z);
            g.DrawFromDef(pos, rot, thingDef, g.GetHashCode());
        }
    }

    public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
    {
        var newLayers = new List<Graphic>(_layers.Count);
        foreach (var g in _layers)
        {
            newLayers.Add(g.GetColoredVersion(newShader, newColor, newColorTwo));
        }

        return new Graphic_Pile(newLayers, _altitudeStep);
    }
}