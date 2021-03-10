using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictionManager : MonoBehaviour
{
    public int maxIterations = 100;
    public int steps = 1;
    public LineRenderer lineRenderer;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    GameObject similuatedObject;

    private void Start() {
        Physics.autoSimulation = false;

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();
    }

	private void FixedUpdate() {
		if(currentPhysicsScene.IsValid()) {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
		}
	}

    public void predict(GameObject subject, Vector3 currentPosition, Vector3 force) {
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid()) {
            if (similuatedObject == null) {
                similuatedObject = Instantiate(subject);
                SceneManager.MoveGameObjectToScene(similuatedObject, predictionScene);
            }

            similuatedObject.transform.position = currentPosition;
            similuatedObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = maxIterations;


            for (int i = 0; i < maxIterations; i++) {
                predictionPhysicsScene.Simulate(Time.fixedDeltaTime * steps);
                lineRenderer.SetPosition(i, similuatedObject.transform.position);
            }

            Destroy(similuatedObject);
        }
    }
}
