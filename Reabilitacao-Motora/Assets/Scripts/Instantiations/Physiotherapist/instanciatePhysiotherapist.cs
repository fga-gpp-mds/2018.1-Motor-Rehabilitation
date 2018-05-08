using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using fisioterapeuta;

public class instanciatePhysiotherapist : MonoBehaviour 
{

	public GameObject buttonPrefab;

	int heightOffset = 0;
	const int HEIGHT_PADDING = 55;

	void ButtonSpawner(int posY, Fisioterapeuta physiotherapist)
	{
		GameObject go = Instantiate(buttonPrefab, transform);

		go.transform.position = new Vector3 (go.transform.position.x + 20, go.transform.position.y - posY, go.transform.position.z);
		
		var script = go.GetComponentInChildren<SetPhysiotherapistToButton>();
		script.Physiotherapist = physiotherapist;

		var temp = go.GetComponentInChildren<Text>();
		temp.text = physiotherapist.persona.nomePessoa;
	}

	public void Awake ()
	{
		List<Fisioterapeuta> physiotherapists = Fisioterapeuta.Read();

		foreach (var physiotherapist in physiotherapists)
		{
			ButtonSpawner(heightOffset, physiotherapist);
			heightOffset += HEIGHT_PADDING;
		}

	}
}