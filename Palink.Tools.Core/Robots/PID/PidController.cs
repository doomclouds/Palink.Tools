using System;

namespace Palink.Tools.Robots.PID;

/// <summary>
/// A (P)roportional, (I)ntegral, (D)erivative Controller
/// </summary>
/// <remarks>
/// The controller should be able to control any process with a
/// measurable value, a known ideal value and an input to the
/// process that will affect the measured value.
/// </remarks>
public sealed class PidController
{
    private double _processVariable;

    public PidController(double gainProportional, double gainIntegral,
        double gainDerivative, double outputMax, double outputMin)
    {
        GainDerivative = gainDerivative;
        GainIntegral = gainIntegral;
        GainProportional = gainProportional;
        OutputMax = outputMax;
        OutputMin = outputMin;
    }

    public TimeSpan LastTime { get; set; }

    /// <summary>
    /// The controller output
    /// </summary>
    /// <param name="timeSinceLastUpdate">timespan of the elapsed time
    /// since the previous time that ControlVariable was called</param>
    /// <returns>Value of the variable that needs to be controlled</returns>
    public double ControlVariable(TimeSpan timeSinceLastUpdate)
    {
        var error = SetPoint - ProcessVariable;

        // integral term calculation
        IntegralTerm += (GainIntegral * error * timeSinceLastUpdate.TotalSeconds);
        IntegralTerm = Clamp(IntegralTerm);

        // derivative term calculation
        var dInput = _processVariable - ProcessVariableLast;
        var derivativeTerm =
            GainDerivative * (dInput / timeSinceLastUpdate.TotalSeconds);
        // proportional term calculation
        var proportionalTerm = GainProportional * error;

        var output = proportionalTerm + IntegralTerm - derivativeTerm;

        output = Clamp(output);

        return output;
    }

    /// <summary>
    /// The derivative term is proportional to the rate of
    /// change of the error
    /// </summary>
    public double GainDerivative { get; set; }

    /// <summary>
    /// The integral term is proportional to both the magnitude
    /// of the error and the duration of the error
    /// </summary>
    public double GainIntegral { get; set; }

    /// <summary>
    /// The proportional term produces an output value that
    /// is proportional to the current error value
    /// </summary>
    /// <remarks>
    /// Tuning theory and industrial practice indicate that the
    /// proportional term should contribute the bulk of the output change.
    /// </remarks>
    public double GainProportional { get; set; }

    /// <summary>
    /// The max output value the control device can accept.
    /// </summary>
    public double OutputMax { get; private set; }

    /// <summary>
    /// The minimum ouput value the control device can accept.
    /// </summary>
    public double OutputMin { get; private set; }

    /// <summary>
    /// Adjustment made by considering the accumulated error over time
    /// </summary>
    /// <remarks>
    /// An alternative formulation of the integral action, is the
    /// proportional-summation-difference used in discrete-time systems
    /// </remarks>
    public double IntegralTerm { get; set; }


    /// <summary>
    /// The current value
    /// </summary>
    public double ProcessVariable
    {
        get => _processVariable;
        set
        {
            ProcessVariableLast = _processVariable;
            _processVariable = value;
        }
    }

    /// <summary>
    /// The last reported value (used to calculate the rate of change)
    /// </summary>
    public double ProcessVariableLast { get; private set; }

    /// <summary>
    /// The desired value
    /// </summary>
    public double SetPoint { get; set; } = 0;

    /// <summary>
    /// Limit a variable to the set OutputMax and OutputMin properties
    /// </summary>
    /// <returns>
    /// A value that is between the OutputMax and OutputMin properties
    /// </returns>
    /// <remarks>
    /// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
    /// </remarks>
    private double Clamp(double variableToClamp)
    {
        if (variableToClamp <= OutputMin)
        {
            return OutputMin;
        }

        return variableToClamp >= OutputMax ? OutputMax : variableToClamp;
    }
}