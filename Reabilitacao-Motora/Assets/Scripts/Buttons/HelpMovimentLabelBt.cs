﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMovimentLabelBt : MonoBehaviour {

    [SerializeField]
    protected Button nextPage;

    public void Awake()
    {
        nextPage.onClick.AddListener(delegate { Flow.StaticHelpMovimentsLabel(); });
    }
}