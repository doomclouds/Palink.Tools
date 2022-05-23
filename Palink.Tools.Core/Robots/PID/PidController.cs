using System;

namespace Palink.Tools.Robots.PID;

[Obsolete("调试过程很难受，最后我还是选择使用简单的SimplePIDController")]
public class PIDController
{
    private double _ts; // Sample period in seconds
    private double _k; // Rollup parameter
    private double _b0, _b1, _b2; // Rollup parameters
    private double _a0, _a1, _a2; // Rollup parameters
    private double _y0; // Current output
    private double _y1; // Output one iteration old
    private double _y2; // Output two iterations old
    private double _e0; // Current error
    private double _e1; // Error one iteration old
    private double _e2; // Error two iterations old

    /// <summary>
    /// PID Constructor
    /// </summary>
    /// <param name="kp">Proportional Gain</param>
    /// <param name="ki">Integral Gain</param>
    /// <param name="kd">Derivative Gain</param>
    /// <param name="n">Derivative Filter Coefficient</param>
    /// <param name="outputUpperLimit">Controller Upper Output Limit</param>
    /// <param name="outputLowerLimit">Controller Lower Output Limit</param>
    public PIDController(double kp, double ki, double kd, double n,
        double outputUpperLimit,
        double outputLowerLimit)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
        N = n;
        OutputUpperLimit = outputUpperLimit;
        OutputLowerLimit = outputLowerLimit;
    }

    /// <summary>
    /// PID iterator, call this function every sample period to get the current controller output.
    /// set point and processValue should use the same units.
    /// </summary>
    /// <param name="setPoint">Current Desired Set Point</param>
    /// <param name="processValue">Current Process Value</param>
    /// <param name="ts">Timespan Since Last Iteration, Use Default Sample Period for First Call</param>
    /// <returns>Current Controller Output</returns>
    public double PIDIterate(double setPoint, double processValue, TimeSpan ts)
    {
        // Ensure the timespan is not too small or zero.
        _ts = (ts.TotalSeconds >= TsMin) ? ts.TotalSeconds : TsMin;

        // Calculate rollup parameters
        _k = 2 / _ts;
        _b0 = Math.Pow(_k, 2) * Kp + _k * Ki + Ki * N + _k * Kp * N +
              Math.Pow(_k, 2) * Kd * N;
        _b1 = 2 * Ki * N - 2 * Math.Pow(_k, 2) * Kp - 2 * Math.Pow(_k, 2) * Kd * N;
        _b2 = Math.Pow(_k, 2) * Kp - _k * Ki + Ki * N - _k * Kp * N +
              Math.Pow(_k, 2) * Kd * N;
        _a0 = Math.Pow(_k, 2) + N * _k;
        _a1 = -2 * Math.Pow(_k, 2);
        _a2 = Math.Pow(_k, 2) - _k * N;

        // Age errors and output history
        _e2 = _e1; // Age errors one iteration
        _e1 = _e0; // Age errors one iteration
        _e0 = setPoint - processValue; // Compute new error
        _y2 = _y1; // Age outputs one iteration
        _y1 = _y0; // Age outputs one iteration
        _y0 = -_a1 / _a0 * _y1 - _a2 / _a0 * _y2 + _b0 / _a0 * _e0 + _b1 / _a0 * _e1 +
              _b2 / _a0 * _e2; // Calculate current output

        // Clamp output if needed
        if (_y0 > OutputUpperLimit)
        {
            _y0 = OutputUpperLimit;
        }
        else if (_y0 < OutputLowerLimit)
        {
            _y0 = OutputLowerLimit;
        }

        return _y0;
    }

    /// <summary>
    /// Reset controller history effectively resetting the controller.
    /// </summary>
    public void ResetController()
    {
        _e2 = 0;
        _e1 = 0;
        _e0 = 0;
        _y2 = 0;
        _y1 = 0;
        _y0 = 0;
    }

    /// <summary>
    /// Proportional Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Kp { get; set; }

    /// <summary>
    /// Integral Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Ki { get; set; }

    /// <summary>
    /// Derivative Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Kd { get; set; }

    /// <summary>
    /// Derivative filter coefficient.
    /// A smaller N for more filtering.
    /// A larger N for less filtering.
    /// Consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double N { get; set; }

    /// <summary>
    /// Minimum allowed sample period to avoid dividing by zero!
    /// The Ts value can be mistakenly set to too low of a value or zero on the first iteration.
    /// TsMin by default is set to 1 millisecond.
    /// </summary>
    public double TsMin { get; set; } = 0.001;

    /// <summary>
    /// Upper output limit of the controller.
    /// This should obviously be a numerically greater value than the lower output limit.
    /// </summary>
    public double OutputUpperLimit { get; set; }

    /// <summary>
    /// Lower output limit of the controller
    /// This should obviously be a numerically lesser value than the upper output limit.
    /// </summary>
    public double OutputLowerLimit { get; set; }
}