using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ImageCaptureHelper : RacecarModule {

  /// <summary>
  /// The width (in pixels) of the color images captured by the camera.
  /// </summary>
  private const int ColorWidth = 640;

  /// <summary>
  /// The height (in pixels) of the color images captured by the camera.
  /// </summary>
  private const int ColorHeight = 480;

  /// <summary>
  /// The average relative error of distance measurements.
  /// Based on the Intel RealSense D435i datasheet.
  /// </summary>
  private const float averageErrorFactor = 0.02f;

  /// <summary>
  /// Time (in ms) to wait for the color or depth image to update during an async call.
  /// </summary>
  private const int asyncWaitTime = 200;

  /// <summary>
  /// The camera that the ImageCaptureHelper object uses.
  /// </summary>
  private Camera camera;

  /// <summary>
  /// Private member for the RawImage accessor.
  /// </summary>
  private byte[] rawImage;

  /// <summary>
  /// True if RawImage is up to date with the color image rendered for the current frame.
  /// </summary>
  public bool isRawImageValid = false;

  /// <summary>
  /// If true, RawImage is updated next frame.
  /// </summary>
  public bool mustUpdateRawImage;

  /// <summary>
  /// Constructor for this class.
  /// </summary>
  public ImageCaptureHelper(Camera camera) {
    this.camera = camera;
    this.rawImage = new byte[sizeof(float) * ImageCaptureHelper.ColorWidth * ImageCaptureHelper.ColorHeight];
  }

  /// <summary>
  /// The GPU-side texture to which the camera renders.
  /// </summary>
  public RenderTexture ImageTexture
  {
      get
      {
          return this.camera.targetTexture;
      }
  }

  /// <summary>
  /// The raw bytes of the color image captured by the camera this frame.
  /// Each pixel is stored in the ARGB 32-bit format, from top left to bottom right.
  /// </summary>
  public byte[] RawImage
  {
      get
      {
          if (!isRawImageValid)
          {
              this.UpdateRawImage();
          }
          return this.rawImage;
      }
  }

  /// <summary>
  /// Asynchronously updates and returns the color image captured by the camera.
  /// Warning: This method blocks for asyncWaitTime ms to wait for the new image to load.
  /// </summary>
  /// <returns>The color image captured by the drone's camera.</returns>
  public byte[] GetRawImageAsync()
  {
      this.mustUpdateRawImage = true;
      Thread.Sleep(ImageCaptureHelper.asyncWaitTime);
      return this.rawImage;
  }

  /// <summary>
  /// Update rawImage by rendering the color camera on the GPU and copying to the CPU.
  /// Warning: this operation is very expensive.
  /// </summary>
  public void UpdateRawImage()
  {
      RenderTexture activeRenderTexture = RenderTexture.active;

      // Tell GPU to render the image captured by the camera
      RenderTexture.active = this.ImageTexture;
      this.camera.Render();

      // Copy this image from the GPU to a Texture2D on the CPU
      Texture2D image = new Texture2D(this.ImageTexture.width, this.ImageTexture.height);
      image.ReadPixels(new Rect(0, 0, this.ImageTexture.width, this.ImageTexture.height), 0, 0);
      image.Apply();

      // Restore the previous GPU render target
      RenderTexture.active = activeRenderTexture;

      // Copy the bytes from the Texture2D to this.colorImageRaw, reversing row order
      // (Unity orders bottom-to-top, we want top-to-bottom)
      byte[] bytes = image.GetRawTextureData();
      int bytesPerRow = ImageCaptureHelper.ColorWidth * 4;
      for (int r = 0; r < ImageCaptureHelper.ColorHeight; r++)
      {
          Buffer.BlockCopy(bytes, (ImageCaptureHelper.ColorHeight - r - 1) * bytesPerRow, this.rawImage, r * bytesPerRow, bytesPerRow);
      }

      Destroy(image);
      this.isRawImageValid = true;
  }

}
