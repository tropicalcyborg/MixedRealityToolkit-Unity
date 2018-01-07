﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class GenericInputPointingSource : GenericInputSource, IPointingSource
    {
        public GenericInputPointingSource(uint sourceId, string name) : base(sourceId, name) { }

        public Cursor Cursor { get; set; }

        public bool InteractionEnabled { get { return true; } }

        public bool FocusLocked { get; set; }

        public float? ExtentOverride { get; set; }

        public RayStep[] Rays { get { return rays; } }
        private readonly RayStep[] rays = { new RayStep(Vector3.zero, Vector3.forward) };

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public bool OwnsInput(BaseEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                   && (inputData.InputSource == this)
                   && (inputData.SourceId == SourceId);
        }

        public bool InputIsFromSource(InputEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                && (inputData.InputSource == this)
                && (inputData.SourceId == SourceId);
        }

        public virtual void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                rays[0].CopyRay(pointingRay, FocusManager.Instance.GetPointingExtent(this));
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(rays[0].origin, rays[0].direction);
                rays[0].CopyRay(RayStabilizer.StableRay, FocusManager.Instance.GetPointingExtent(this));
            }
        }

        public virtual void OnPostRaycast() { }

        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public virtual bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = default(Ray);
            return false;
        }

        public virtual bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }
    }
}