//-----------------------------------------------------------------------
// <copyright file="AugmentedImageExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.AugmentedImage
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controller for AugmentedImage example.
    /// </summary>
    /// <remarks>
    /// In this sample, we assume all images are static or moving slowly with
    /// a large occupation of the screen. If the target is actively moving,
    /// we recommend to check <see cref="AugmentedImage.TrackingMethod"/> and
    /// render only when the tracking method equals to
    /// <see cref="AugmentedImageTrackingMethod.FullTracking"/>.
    /// See details in <a href="https://developers.google.com/ar/develop/c/augmented-images/">
    /// Recognize and Augment Images</a>
    /// </remarks>
    public class AugmentedImageItemController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an AugmentedImage.
        /// </summary>
        public AugmentedItemVisualizer AugmentedImageVisualizerPrefab;

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        //public GameObject FitToScanOverlay;

        private Dictionary<int, AugmentedItemVisualizer> m_Visualizers
            = new Dictionary<int, AugmentedItemVisualizer>();

        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

        public Text test_text;

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(
                m_TempAugmentedImages, TrackableQueryFilter.Updated);

            foreach (var image in m_TempAugmentedImages)
            {
                AugmentedItemVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);


                if (image.TrackingState == TrackingState.Tracking && visualizer == null)
                {
                    if (image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
                    {
                        Anchor anchor = image.CreateAnchor(image.CenterPose);
                        visualizer = (AugmentedItemVisualizer)Instantiate(
                                      AugmentedImageVisualizerPrefab, anchor.transform);
                        visualizer.Image = image;

                        m_Visualizers.Add(image.DatabaseIndex, visualizer);

                        test_text.text = image.Name + " detected";

                        visualizer.gameObject.name = image.Name;

                        setItemData(visualizer.gameObject, int.Parse(image.Name));
                    }
                }
                else
                {
                    if (image.TrackingMethod == AugmentedImageTrackingMethod.LastKnownPose)
                    {
                        m_Visualizers.Remove(image.DatabaseIndex);
                        GameObject.Destroy(visualizer.gameObject);
                    }
                }
            }
        }

        public void setItemData(GameObject obj,int item_num)
        {
            CanvasManager cm = obj.GetComponentInChildren<CanvasManager>();

            //var ij = LoadJSONfromGAS.Instance.getItem(item_num);

            var ij = PrismDBItemListGetter.Instance.getItem(item_num);

            //int possession_num = ij.possession_num;

            string item_id = ij.id.value;

            int possession_num = PUFirebaseTwitterLogin.Instance.getItemPosessionNum(item_id);
            Debug.Log("setItemData " + possession_num);

            string coord_name = ij.name.value;

            //if (ij.name2 != "")
            //    coord_name += "\n" + ij.name2;

            cm.setCoordName(coord_name);

            cm.setPosessionNum(possession_num);

            if (possession_num > 0)
                cm.setHaveStatus(true);
            else
                cm.setHaveStatus(false);

            cm.setItem(ij);
        }
    }
}

