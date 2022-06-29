using UnityEngine;
using System.Collections.Generic;
namespace ChallengeAI {
  public static class Extentions {
    public static T GetRandomItem<T>(this List<T> list) {
      return list[Random.Range(0,list.Count)];
    }
    public static T GetRandomItem<T>(this T[] list) {
      return list[Random.Range(0,list.Length)];
    }
    public static Color From(this Color color, Color from, float alpha) {
      return new Color(from.r,from.g,from.b,alpha);
    }
    public static Color Alpha(this Color color, float alpha) {
      return new Color(color.r,color.g,color.b,alpha);
    }
    public static Vector3 ToXY0(this Vector3 vector3) {
      return new Vector3(vector3.x,vector3.y,0);
    }
    public static Vector3 ToX0Z(this Vector3 vector3) {
      return new Vector3(vector3.x,0,vector3.z);
    }
    public static Vector3 To0YZ(this Vector3 vector3) {
      return new Vector3(0,vector3.y,vector3.z);
    }
    public static Vector2 XY(this Vector3 vector3) {
      return new Vector2(vector3.x,vector3.y);
    }
    public static Vector2 XZ(this Vector3 vector3) {
      return new Vector2(vector3.x,vector3.z);
    }
    public static Vector2 YZ(this Vector3 vector3) {
      return new Vector2(vector3.y,vector3.z);
    }
  }
}