using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPaletteController : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler, IInitializePotentialDragHandler
{
    [SerializeField] private RectTransform _picker;
    [SerializeField] private Image _pickedColorImage;
    [SerializeField] private Material _colorWheelMat;
    [SerializeField] private int _totalNumberofColors = 24;
    [SerializeField] private int _wheelsCount = 2;
    [SerializeField]
    [Range(0,360)]
    [Tooltip("clockwise angle of the begnning point starting from positive x-axis")]
    private float _startingAngle = 0;
    [SerializeField] [InspectorName("Control Sat & Val")] private bool _controlSV = false;
    [SerializeField] private bool _inertia = true;
    [SerializeField] private float _decelerationRate = 0.135f;
    [SerializeField] private bool _wholeSegment = false;

    [Header("Limits")]
    [SerializeField] [Range(0.5f, 0.001f)] private float _minimumSatValStep = 0.01f;
    [SerializeField] [Range(0,1)] private float _minimumSaturation = 0.25f;
    [SerializeField] [Range(0, 1)] private float _maximumSaturation = 1;
    [SerializeField] [Range(0, 1)] private float _minimumValue = 0.25f;
    [SerializeField] [Range(0, 1)] private float _maximumValue = 1;
    
    //dragging variables
    private bool _dragging = false;
    private float _satValAmount = 1;
    private float _omega = 0;
    private float _previousTheta;
    private float _theta;

    private float _previousDiscretedH;
    private float _sat = 1, _val = 1;
    private Color _selectedColor;

    public Color SelectedColor {
        get
        {
            return _selectedColor;
        }
        private set
        {
            if(value!=_selectedColor)
            {
                _selectedColor = value;
                OnColorChange?.Invoke(SelectedColor);
            }
        }
    }

    public float Hue { get; private set; } = 0;

    public float Value
    {
        get { return _val; }
        set
        {
            float newVal = Mathf.Clamp(value, _minimumValue, _maximumValue);
            if(Mathf.Abs(_val- newVal) > _minimumSatValStep)
            {
                _val = newVal;
                UpdateMaterial();
                UpdateColor();
            }
        }
    }

    public float Saturation
    {
        get { return _sat; }
        set
        {            
            float newSat = Mathf.Clamp(value, _minimumSaturation, _maximumSaturation);
            if (Mathf.Abs(_sat - newSat) > _minimumSatValStep)
            {
                _sat = newSat;
                UpdateMaterial();
                UpdateColor();
            }
        }
    }

    public ColorChangeEvent OnColorChange;
    public HueChangeEvent OnHueChange;

    public void CalculateColorPallete()
    {
        CalculatePresets();
        UpdateMaterialInitialValues();
        UpdateMaterial();
        UpdateColor();
    }

    private void Awake()
    {
        CalculateColorPallete();
    }

    private void UpdateMaterialInitialValues()
    {
        _colorWheelMat.SetFloat("_StartingAngle", _startingAngle);
        _colorWheelMat.SetInt("_ColorsCount" , _totalNumberofColors);
        _colorWheelMat.SetInt("_WheelsCount", _wheelsCount);
    }

    //presets
    private Vector2 _centerPoint;
    private float _paletteRadius;
    private float _pickerHueOffset;
    //if the palette position/size or Picker position change in runtime, this function should be called 
    private void CalculatePresets()
    {
        //Assuming the canvas is ScreenSpace-Overlay
        _centerPoint=RectTransformUtility.WorldToScreenPoint(null, transform.position);
        RectTransform rect = GetComponent<RectTransform>();
        _paletteRadius = rect.sizeDelta.x / 2;
        Vector3 pickerLocalPosition = _picker.localPosition;
        float angle = Vector2.SignedAngle(Vector2.right, pickerLocalPosition);
        if (angle < 0) angle += 360;
        _pickerHueOffset = angle / 360;

    }

    public void CalculateSaturationAndValue(float amount)
    {
       
        if(amount>1)
        {
            _val = 1;
            _sat = 2-amount ;
        }
        else
        {
            _sat = 1;
            _val = amount;
        }

        _sat = Mathf.Clamp(_sat, _minimumSaturation, _maximumSaturation);
        _val = Mathf.Clamp(_val, _minimumValue, _maximumValue);
        UpdateMaterial();
        UpdateColor();
    }

    public void UpdateHue()
    {
        UpdateMaterial();
        UpdateColor();
    }

    public void UpdateMaterial()
    {
        //if wholeSegment is checked, we should make sure to update the material with incremental value that is equal to segment size
        if (_wholeSegment)
        {
            float discretedHue = ((int)((Hue + _startingAngle / 360.0f) * _totalNumberofColors)) / (1.0f * (_totalNumberofColors));
            _colorWheelMat.SetFloat("_Hue", discretedHue);
        }
        else
        {
            _colorWheelMat.SetFloat("_Hue", Hue);
        }
        if (_controlSV)
        {
            _colorWheelMat.SetFloat("_Sat", _sat);
            _colorWheelMat.SetFloat("_Val", _val);
        }

    }

    public void UpdateColor()
    {
        
        float shiftedH = (_pickerHueOffset + _startingAngle / 360.0f + Hue % _wheelsCount) / _wheelsCount;
        shiftedH = shiftedH % 1.0f;
        float discretedH = ((int)(shiftedH * _totalNumberofColors)) / (1.0f* (_totalNumberofColors-1));
        Color color;
        if (shiftedH > 1 - 1.0 / _totalNumberofColors  && shiftedH <= 1)//for gray
            color = Color.HSVToRGB(0, 0, (_val - _sat + 0.75f) / 1.5f);
        else
            color = Color.HSVToRGB(discretedH, _sat, _val);
        if (_previousDiscretedH != discretedH)
            OnHueChange?.Invoke(discretedH);
        if(_pickedColorImage) _pickedColorImage.color = color;
        SelectedColor = color;
        _previousDiscretedH = discretedH;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector2 dragVec = eventData.delta;
        Vector2 currentPos = eventData.position;
        Vector2 prevPos = currentPos - dragVec;
        
        //calculate Saturation and Value change
        if (_controlSV)
        {
            float r1 = Vector2.Distance(_centerPoint, prevPos);
            float r2 = Vector2.Distance(_centerPoint, currentPos);
            float dr = r2 - r1;
            _satValAmount += dr / _paletteRadius;
            _satValAmount = Mathf.Clamp(_satValAmount, 0, 2);
            CalculateSaturationAndValue(_satValAmount);
        }

        //calculate Hue change
        float dtheta = Vector2.SignedAngle(currentPos - _centerPoint,prevPos - _centerPoint);
        _theta += dtheta;

        Hue += dtheta / 360;
        if (Hue < 0) 
            Hue += _wheelsCount;
        
        UpdateHue();

    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _omega = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _dragging = true;
        _omega = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _dragging = false;
    }

    public void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        if (_dragging && _inertia)
        {
            float newOmega = (_theta - _previousTheta) / Time.deltaTime;
            _omega = Mathf.Lerp(_omega, newOmega, deltaTime * 10);
            _previousTheta = _theta;
        }

        if (!_dragging && _omega != 0)
        {
            _omega *= Mathf.Pow(_decelerationRate, deltaTime);
            if (Mathf.Abs(_omega) < 1)
                _omega = 0;
            float dtheta = _omega * deltaTime;
            Hue += dtheta / 360;
            if (Hue < 0) Hue += _wheelsCount;
            UpdateHue();
        }
    }
}

[System.Serializable]
public class ColorChangeEvent : UnityEvent<Color>
{}
[System.Serializable]
public class HueChangeEvent : UnityEvent<float>
{ }
