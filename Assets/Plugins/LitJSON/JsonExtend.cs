using System;
using System.Collections.Generic;
using UnityEngine;

namespace LitJson
{
    public class JsonExtend
    {
        public static void AddExtends()
        {
            // Vector4 exporter
            ExporterFunc<Vector4> vector4Exporter = new ExporterFunc<Vector4>(ExportVector4);
            JsonMapper.RegisterExporter<Vector4>(vector4Exporter);

            // Vector3 exporter
            ExporterFunc<Vector3> vector3Exporter = new ExporterFunc<Vector3>(ExportVector3);
            JsonMapper.RegisterExporter<Vector3>(vector3Exporter);

            // Vector2 exporter
            ExporterFunc<Vector2> vector2Exporter = new ExporterFunc<Vector2>(ExportVector2);
            JsonMapper.RegisterExporter<Vector2>(vector2Exporter);
			
			ExporterFunc<Quaternion> quaternionExporter = new ExporterFunc<Quaternion>(ExportQuaternion);
            JsonMapper.RegisterExporter<Quaternion>(quaternionExporter);
			
//            // float to double
//            ExporterFunc<float> float2double = new ExporterFunc<float>(JsonExtend.float2double);
//            JsonMapper.RegisterExporter<float>(float2double);
//
//            // double to float
//            ImporterFunc<double, Single> double2float = new ImporterFunc<double, Single>(JsonExtend.double2float);
//            JsonMapper.RegisterImporter<double, Single>(double2float);
        }
		
		public static void ExportQuaternion(Quaternion value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WritePropertyName("w");
            writer.Write(value.w);
            writer.WriteObjectEnd();
        }
		
        public static void ExportVector4(Vector4 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WritePropertyName("w");
            writer.Write(value.w);
            writer.WriteObjectEnd();
        }

        public static void ExportVector3(Vector3 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WriteObjectEnd();
        }

        public static void ExportVector2(Vector2 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WriteObjectEnd();
        }

//        public static void float2double(float value, JsonWriter writer)
//        {
//            writer.Write((double)value);
//        }
//
//        public static System.Single double2float(double value)
//        {
//            return (System.Single)value;
//        }
    }
}