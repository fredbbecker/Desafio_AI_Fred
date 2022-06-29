namespace ChallengeAI {
  public delegate void GenericDelegate<T>(T arg0);
  [System.Serializable]
  public class ObservableProperty<T> {
    private T _property;
    private System.Func<T,T> _validate;
    public GenericDelegate<T> OnChange;
    public T Value {
      get => _property;
      set {
        _property = _validate != null ? _validate(value) : value;
        OnChange?.Invoke(_property);
      }
    }

    public ObservableProperty() {}
    public ObservableProperty(T initialValue) {
      _property = initialValue;
    }
    public ObservableProperty(System.Func<T,T> validate) {
      _validate = validate;
    }
    public ObservableProperty(T initialValue, System.Func<T,T> validate) {
      _property = initialValue;
      _validate = validate;
    }

    public override string ToString()
    {
      return Value.ToString();
    }
  }
}