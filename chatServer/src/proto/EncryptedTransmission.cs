// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: encryptedTransmission.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ChatServer.Transmissions {

  /// <summary>Holder for reflection information generated from encryptedTransmission.proto</summary>
  public static partial class EncryptedTransmissionReflection {

    #region Descriptor
    /// <summary>File descriptor for encryptedTransmission.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static EncryptedTransmissionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChtlbmNyeXB0ZWRUcmFuc21pc3Npb24ucHJvdG8SGENoYXRTZXJ2ZXIuVHJh",
            "bnNtaXNzaW9ucyI2ChVFbmNyeXB0ZWRUcmFuc21pc3Npb24SHQoVRW5jcnlw",
            "dGVkVHJhbnNtaXNzaW9uGAEgASgMYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ChatServer.Transmissions.EncryptedTransmission), global::ChatServer.Transmissions.EncryptedTransmission.Parser, new[]{ "EncryptedTransmission_" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class EncryptedTransmission : pb::IMessage<EncryptedTransmission>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<EncryptedTransmission> _parser = new pb::MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<EncryptedTransmission> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ChatServer.Transmissions.EncryptedTransmissionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public EncryptedTransmission() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public EncryptedTransmission(EncryptedTransmission other) : this() {
      encryptedTransmission_ = other.encryptedTransmission_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public EncryptedTransmission Clone() {
      return new EncryptedTransmission(this);
    }

    /// <summary>Field number for the "EncryptedTransmission" field.</summary>
    public const int EncryptedTransmission_FieldNumber = 1;
    private pb::ByteString encryptedTransmission_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString EncryptedTransmission_ {
      get { return encryptedTransmission_; }
      set {
        encryptedTransmission_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as EncryptedTransmission);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(EncryptedTransmission other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (EncryptedTransmission_ != other.EncryptedTransmission_) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (EncryptedTransmission_.Length != 0) hash ^= EncryptedTransmission_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (EncryptedTransmission_.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(EncryptedTransmission_);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (EncryptedTransmission_.Length != 0) {
        output.WriteRawTag(10);
        output.WriteBytes(EncryptedTransmission_);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (EncryptedTransmission_.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(EncryptedTransmission_);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(EncryptedTransmission other) {
      if (other == null) {
        return;
      }
      if (other.EncryptedTransmission_.Length != 0) {
        EncryptedTransmission_ = other.EncryptedTransmission_;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            EncryptedTransmission_ = input.ReadBytes();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            EncryptedTransmission_ = input.ReadBytes();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
