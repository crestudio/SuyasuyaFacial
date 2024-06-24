#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using VRC.SDK3.Avatars.Components;

/*
 * VRSuya Suyasuya Facial
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.suyasuyafacial {

	[Serializable]
	public struct FaceBlendShape {
		public bool ActiveValue;
		public int IndexValue;
		public string BlendShapeName;
		public float BlendShapeValue;

		public FaceBlendShape(bool BoolValue, int IntValue, string StringName, float FloatValue) {
			ActiveValue = BoolValue;
			IndexValue = IntValue;
			BlendShapeName = StringName;
			BlendShapeValue = FloatValue;
		}
	};

	[ExecuteInEditMode]
	[AddComponentMenu("VRSuya Suyasuya Facial")]
	public class SuyasuyaFacial : MonoBehaviour {

        // 아바타 에디터용 변수
        public GameObject AvatarGameObject;
		public SkinnedMeshRenderer AvatarHeadSkinnedMeshRenderer;
		public AnimationClip[] TargetAnimationClips = new AnimationClip[0];
		public FaceBlendShape[] TargetBlendShapes = new FaceBlendShape[0];
		
		// 상태 반환
		public string StatusCode = "";
		public int UndoGroupIndex;

		// 컴포넌트 최초 로드시 동작
		void OnEnable() {
			if (!AvatarGameObject) AvatarGameObject = this.gameObject;
			AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
			if (AvatarHeadSkinnedMeshRenderer) {
				TargetBlendShapes = GetAvatarBlendShapeStatus(AvatarHeadSkinnedMeshRenderer);
			}
			if (TargetAnimationClips.Length == 0) TargetAnimationClips = GetVRSuyaSuyasuyaAnimations();
		}

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void UpdateAnimationClips() {
			Undo.IncrementCurrentGroup();
			Undo.SetCurrentGroupName("VRSuya Suyasuya Facial");
			UndoGroupIndex = Undo.GetCurrentGroup();
			if (VerifyVariable()) {
				
				Debug.Log("[VRSuya Suyasuya Facial] Update Animation Clips Completed");
            }
			return;
        }

		/// <summary>변수를 초기화하고 다시 불러옵니다.</summary>
		public void ReloadVariable() {
			TargetBlendShapes = new FaceBlendShape[0];
			if (!AvatarGameObject) AvatarGameObject = this.gameObject;
			if (!AvatarHeadSkinnedMeshRenderer) {
				AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
			}
			if (AvatarHeadSkinnedMeshRenderer) {
				TargetBlendShapes = GetAvatarBlendShapeStatus(AvatarHeadSkinnedMeshRenderer);
			}
			if (TargetAnimationClips.Length == 0) TargetAnimationClips = GetVRSuyaSuyasuyaAnimations();
			return;
		}

		/// <summary>아바타의 현재 상태를 검사하여 패치가 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		private bool VerifyVariable() {
			if (AvatarHeadSkinnedMeshRenderer) {
				return true;
			} else {
				if (AvatarGameObject) {
					AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
					if (AvatarHeadSkinnedMeshRenderer) {
						return true;
					} else {
						StatusCode = "NO_FACEMESH";
						return false;
					}
				} else {
					StatusCode = "NO_AVATAR";
					return false;
				}
			}
        }

		/// <summary>아바타의 Head 스킨드 메쉬 렌더러를 찾습니다.</summary>
		/// <returns>아바타의 Head 스킨드 메쉬 렌더러</returns>
		private SkinnedMeshRenderer GetAvatarHeadSkinnedMeshRenderer(GameObject TargetGameObject) {
			AvatarGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
			if (AvatarDescriptor) {
				return AvatarDescriptor.GetComponent<VRCAvatarDescriptor>().VisemeSkinnedMesh;
			}
			return null;
		}

		/// <summary>아바타에서 적용된 Blendshape 리스트를 반환합니다.</summary>
		/// <returns>활성화 아바타 BlendShape 어레이</returns>
		private FaceBlendShape[] GetAvatarBlendShapeStatus(SkinnedMeshRenderer TargetSkinnedMeshRenderer) {
			FaceBlendShape[] newFaceBlendShapeList = new FaceBlendShape[0];
			if (TargetSkinnedMeshRenderer.sharedMesh.blendShapeCount > 0) {
				for (int Index = 0; Index < TargetSkinnedMeshRenderer.sharedMesh.blendShapeCount; Index++) {
					if (TargetSkinnedMeshRenderer.GetBlendShapeWeight(Index) > 0) {
						FaceBlendShape TargetBlendShape = new FaceBlendShape(
							false,
							Index,
							TargetSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(Index),
							TargetSkinnedMeshRenderer.GetBlendShapeWeight(Index)
						);
						newFaceBlendShapeList = newFaceBlendShapeList.Concat(new FaceBlendShape[] { TargetBlendShape }).ToArray();
					}
				}
			}
			return newFaceBlendShapeList;
		}

		/// <summary>에셋에서 Suyasuya 애니메이션을 찾아서 리스트로 반환 합니다.</summary>
		/// <returns>Suyasuya 애니메이션 클립 어레이</returns>
		private AnimationClip[] GetVRSuyaSuyasuyaAnimations() {
			Dictionary<string,string> dictSuyasuyaAnimationClips = new Dictionary<string, string> {
				{ "3e14bdc27b543fc4d9c16398be0a9d9c", "VRSuya_Suyasuya_Animation_50_Sleeping_Animation" },
				{ "b6c63881d1b4cc740a95793be2b0ff7f", "VRSuya_Suyasuya_Animation_50_Sleeping_NoAnimation" },
				{ "fd098f6d505fb1749971fc83d7d4b6c1", "VRSuya_Suyasuya_Animation_50_WakeUp" },
				{ "70b313f6f6d5c2746a2a78c8ab7254d5", "VRSuya_Suyasuya_Animation_100_Sleeping_Animation" },
				{ "3809f2986699be140a19aa19b1bf40b8", "VRSuya_Suyasuya_Animation_100_Sleeping_NoAnimation" },
				{ "22426a8e76770e84da9c2c1d2497dadd", "VRSuya_Suyasuya_Animation_100_WakeUp"}
			};
			AnimationClip[] newAnimationClips = new AnimationClip[0];
			foreach (KeyValuePair<string, string> dictTargetAnimationClip in dictSuyasuyaAnimationClips) {
				if (!String.IsNullOrEmpty(GUIDToAssetName(dictTargetAnimationClip.Key, true))) {
					AnimationClip TargetAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(dictTargetAnimationClip.Key));
					if (!TargetAnimationClip.empty) {
						newAnimationClips = newAnimationClips.Concat(new AnimationClip[] { TargetAnimationClip }).ToArray();
					}
				}
			}
			return newAnimationClips;
		}

		/// <summary>요청한 GUID를 파일 이름으로 반환합니다. 2번째 인자는 확장명 포함 여부를 결정합니다.</summary>
		/// <returns>파일 이름</returns>
		private string GUIDToAssetName(string GUID, bool OnlyFileName) {
			string FileName = "";
			FileName = AssetDatabase.GUIDToAssetPath(GUID).Split('/')[AssetDatabase.GUIDToAssetPath(GUID).Split('/').Length - 1];
			if (OnlyFileName) FileName = FileName.Split('.')[0];
			return FileName;
		}
	}
}
#endif