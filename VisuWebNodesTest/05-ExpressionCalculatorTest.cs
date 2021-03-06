﻿using System;
using System.Collections;
using System.Collections.Generic;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;

namespace Recomedia_de.Logic.VisuWeb.Test
{
  [TestFixture]
  public class ExpressionCalculatorTest : PlaceholderTestBase<ExpressionCalculator>
  {
    public ExpressionCalculatorTest() : base() { }

    [SetUp]
    public void ExpressionCalculatorTestSetUp()
    {
      node = new ExpressionCalculator(context);
    }

    [Test]
    public void StringBool()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Bool;
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses a few string placeholders
      node.mTemplates[0].Value = "{a:S} == {b:S}";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 0, 2);
      checkInputNames<BoolValueObject>  (new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { }, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { "a", "b" },
                                                               node.mStrInputs);
      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mStrInputs[0].Value = "x";
      node.mStrInputs[0].WasSet = false;  // assume this as a start value
      node.mStrInputs[1].Value = "y";
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(false, node.mOutputs[0].Value);

      // Change an input value and re-check the output
      node.mStrInputs[0].Value = "y";
      node.mStrInputs[1].WasSet = false;  // no new value
      node.Execute();
      Assert.AreEqual(true, node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleString()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.String;
      Assert.AreEqual(PortTypes.String, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses a few string placeholders
      node.mTemplates[0].Value = "{a:S} + {b:S}";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 0, 2);
      checkInputNames<BoolValueObject>(new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>(new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { }, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { "a", "b" },
                                                               node.mStrInputs);
      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be string
      Assert.AreEqual(PortTypes.String, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mStrInputs[0].Value = "x";
      node.mStrInputs[0].WasSet = false;  // assume this as a start value
      node.mStrInputs[1].Value = "y";
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual("xy", node.mOutputs[0].Value);

      // Change an input value and re-check the output
      node.mStrInputs[0].Value = "y";
      node.mStrInputs[1].WasSet = false;  // no new value
      node.Execute();
      Assert.AreEqual("yy", node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleDateString()
    {
      // Use two valid expressions
      node.mTemplateCount.Value = 2;
      Assert.AreEqual(2, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.String;
      node.mOutputTypes[1].Value = PortTypes.String;
      Assert.AreEqual(PortTypes.String, node.mOutputs[0].PortType.Name);
      Assert.AreEqual(PortTypes.String, node.mOutputs[1].PortType.Name);

      // Set a simple valid template that uses a few string placeholders
      node.mTemplates[0].Value = "(new DateTime(2019,2,27)).AddDays({PlusTage:N}).ToString(\"d\")";
      node.mTemplates[1].Value = "{Vortext:S} + \": \" + _out1_";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 1, 1);
      checkInputNames<BoolValueObject>(new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>(new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { "PlusTage" }, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { "Vortext" }, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be string
      Assert.AreEqual(PortTypes.String, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);         // should be string
      Assert.AreEqual(PortTypes.String, node.mOutputs[1].PortType.Name);
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mStrInputs[0].Value = "Morgen";
      node.mNumInputs[0].Value = 1.0;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual("28.02.2019", node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual("Morgen: 28.02.2019", node.mOutputs[1].Value);

      // Change an input value and re-check the output
      node.mStrInputs[0].Value = "Übermorgen";
      node.mNumInputs[0].Value = 2.0;
      node.Execute();
      Assert.AreEqual("01.03.2019", node.mOutputs[0].Value);
      Assert.AreEqual("Übermorgen: 01.03.2019", node.mOutputs[1].Value);
    }

    [Test]
    public void SimpleBool()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Bool;
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses a few placeholders
      node.mTemplates[0].Value = "({a:B} && {b:B}) || ({c:B} && {d:B})";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(4, 0, 0, 0);
      checkInputNames<BoolValueObject>  (new List<string>{ "a", "b", "c", "d" },
                                                             node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{}, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{}, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mBinInputs[0].Value = true;
      node.mBinInputs[0].WasSet = false;  // assume this as a start value
      node.mBinInputs[1].Value = true;
      node.mBinInputs[2].Value = false;
      node.mBinInputs[2].WasSet = false;  // assume this as a start value
      node.mBinInputs[3].Value = false;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mBinInputs[0].Value = true;
      node.mBinInputs[1].Value = false;
      node.mBinInputs[2].Value = false;
      node.mBinInputs[3].Value = true;

      node.Execute();

      bool isTimeTest = false;
      Stopwatch first1000 = new Stopwatch();
      if (isTimeTest)
      {
        first1000.Start();
        for (int i = 0; i < 1000; i++)
        {
          node.Execute();
        }
        first1000.Stop();
      }
      TimeSpan tsFirst1000 = first1000.Elapsed;

      if (isTimeTest)
      {
        for (int i = 0; i < 4998000; i++)
        {
          node.Execute();
        }
      }

      Stopwatch last1000 = new Stopwatch();
      if (isTimeTest)
      {
        last1000.Start();
        for (int i = 0; i < 1000; i++)
        {
          node.Execute();
        }
        last1000.Stop();
      }
      TimeSpan tsLast1000 = last1000.Elapsed;

      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(false, node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleByte()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Byte;
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses a few placeholders
      node.mTemplates[0].Value = "Light.RGBToR({rgb:I})";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 1, 0, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "rgb" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{}, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be byte
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mIntInputs[0].Value = 0x579bdf;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0x57, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mIntInputs[0].Value = 0xb97531;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(0xb9, node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleInteger()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Integer;
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses one placeholder
      node.mTemplates[0].Value = "{n:I} + Math.Abs((int)Math.Round({x:F}))";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);
      // Check the resulting inputs
      checkInputCounts(0, 1, 1, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "n" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{ "x" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be int
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mIntInputs[0].Value = 13;
      node.mNumInputs[0].Value = 8.5000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(22, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mIntInputs[0].Value = 17;
      node.mNumInputs[0].Value = -3.4999999;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(20, node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleLong()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Int64;
      Assert.AreEqual(PortTypes.Int64, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses one placeholder
      node.mTemplates[0].Value = "Light.RGBW({r:I}, {g:I}, {b:I}, {w:I})";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);
      // Check the resulting inputs
      checkInputCounts(0, 4, 0, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "r", "g", "b", "w" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{}, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be long aka int64
      Assert.AreEqual(PortTypes.Int64, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mIntInputs[0].Value = 0xed;
      node.mIntInputs[1].Value = 0x87;
      node.mIntInputs[2].Value = 0xba;
      node.mIntInputs[3].Value = 0x54;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0xfed87ba54, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mIntInputs[0].Value = 0xde;
      node.mIntInputs[1].Value = -1;
      node.mIntInputs[2].Value = 0xab;
      node.mIntInputs[3].Value = -1;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(0xade00ab00, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mIntInputs[0].Value = 256;
      node.mIntInputs[1].Value = 0x78;
      node.mIntInputs[2].Value = 256;
      node.mIntInputs[3].Value = 0x45;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(0x500780045, node.mOutputs[0].Value);
    }

    [Test]
    public void SimpleNumeric()
    {
      // Use the default template; don't set the number of templates
      Assert.AreEqual(1, node.mTemplates.Count);

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Number;
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses just one placeholder
      node.mTemplates[0].Value = "Math.Pow(2*{x:N},2)";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 1, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{}, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{ "x" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set an input value and re-check the output
      node.mNumInputs[0].Value = 1.5;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(9, node.mOutputs[0].Value); // (2*1.5)^2
    }

    [Test]
    public void MultiConditionalMonolithic()
    {
      // Use the default template and output type
      Assert.AreEqual(1, node.mTemplates.Count);
      node.mTemplates[0].Value =
        "({T:f} > 20.0) ? 0 : (" +          // T > 20°C ==> 0°C
        "  ({T} > 11.0) ? (" +
        "    21.0 + (20.0-{T})*19.0/9.0" +  // T = 20°C .. 11°C ==> 21°C .. 40°C
        "  ) : (" +
        "    ({T} > -10.0) ? (" +           // T = 11°C ..-10°C ==> 40°C .. 70°C
        "      40.0 + (11.0-{T})*30.0/21.0" +
        "    ) : 70.0" +                    // T < -10°C ==> 70°C
        "  )" +
        ")";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 1, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{}, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{ "T" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set an input value and re-check the output
      node.mNumInputs[0].Value = 20.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0.0, node.mOutputs[0].Value);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = 19.999999;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(21.0, (double)node.mOutputs[0].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = 11.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(40.0, (double)node.mOutputs[0].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = -9.999999;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(70.0, (double)node.mOutputs[0].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = -10.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(70.0, node.mOutputs[0].Value);
    }

    [Test]
    public void MultiConditionalRefs()
    {
      // Use three valid expressions
      node.mTemplateCount.Value = 3;
      Assert.AreEqual(3, node.mTemplates.Count);
      node.mTemplates[0].Value =
        "({T:f} > -10.0) ? (" +           // T = 11°C ..-10°C ==> 40°C .. 70°C
        "  40.0 + (11.0-{T})*30.0/21.0" +
        ") : 70.0";                       // T < -10°C ==> 70°C
      node.mTemplates[1].Value =
        "({T} > 11.0) ? (" +
        "  21.0 + (20.0-{T})*19.0/9.0" +  // T = 20°C .. 11°C ==> 21°C .. 40°C
        ") : _out1_";
      node.mTemplates[2].Value =
        "({T} > 20.0) ? 0 : _out2_";      // T > 20°C ==> 0°C

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 1, 0);
      checkInputNames<BoolValueObject>(new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>(new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { "T" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { }, node.mStrInputs);

      // Check the output states
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);         // should be double
      Assert.AreEqual(PortTypes.Number, node.mOutputs[1].PortType.Name);
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[2]);         // should be double
      Assert.AreEqual(PortTypes.Number, node.mOutputs[2].PortType.Name);
      Assert.IsFalse(node.mOutputs[2].HasValue);  // no output value yet

      // Set an input value and re-check the output
      node.mNumInputs[0].Value = 20.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0.0, node.mOutputs[2].Value);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = 19.999999;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);
      Assert.AreEqual(21.0, (double)node.mOutputs[2].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = 11.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);
      Assert.AreEqual(40.0, (double)node.mOutputs[2].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = -9.999999;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);
      Assert.AreEqual(70.0, (double)node.mOutputs[2].Value, 3e-6);

      // Set another input value and re-check the output
      node.mNumInputs[0].Value = -10.0000001;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);
      Assert.AreEqual(70.0, node.mOutputs[2].Value);
    }

    [Test]
    public void MultiCompare()
    {
      // Use four valid expressions
      node.mTemplateCount.Value = 4;
      Assert.AreEqual(4, node.mTemplates.Count);
      node.mOutputTypes[0].Value = PortTypes.Bool;
      node.mTemplates[0].Value = "{a:I}=={b:I}";
      node.mTemplates[1].Value = "{a:I} != {b:I}";
      node.mTemplates[2].Value = "{a:I} >={b:I}";
      node.mTemplates[3].Value = "{a:I}<= {b:I}";
      node.mOutputTypes[0].Value = PortTypes.Bool;
      node.mOutputTypes[1].Value = PortTypes.Bool;
      node.mOutputTypes[2].Value = PortTypes.Bool;
      node.mOutputTypes[3].Value = PortTypes.Bool;

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 2, 0, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "a", "b" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{}, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output states
      Assert.IsNotNull(node.mOutputs[0]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[1].PortType.Name);
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[2]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[2].PortType.Name);
      Assert.IsFalse(node.mOutputs[2].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[3]);         // should be bool
      Assert.AreEqual(PortTypes.Bool, node.mOutputs[3].PortType.Name);
      Assert.IsFalse(node.mOutputs[3].HasValue);  // no output value yet

      // Set an input value and re-check the outputs
      node.mIntInputs[0].Value = 3;
      node.mIntInputs[1].Value = 3;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(false, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[3].Value);

      // Change one of the input values and re-check the outputs
      node.mIntInputs[0].WasSet = false;  // no new value
      node.mIntInputs[1].Value = 4;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(false, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(false, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(true, node.mOutputs[3].Value);
    }

    [Test]
    public void DegRadFunctionsAndConstants()
    {
      // Use four valid expressions
      node.mTemplateCount.Value = 4;
      Assert.AreEqual(4, node.mTemplates.Count);
      node.mTemplates[0].Value = "Angle.Deg({rad:N})";
      node.mTemplates[1].Value = "Math.Sin(Angle.Rad({deg:N}))";
      node.mTemplates[2].Value = "(double)({n:I}) * Math.PI";
      node.mTemplates[3].Value = "Math.E / {x:N}";

      // Check template and output names
      Assert.AreEqual("Anzahl der Ausgänge und Formeln",
                      node.Localize("de", node.mTemplateCount.Name));
      checkDeLocalizedList<EnumValueObject>(
        new List<string> { "Typ des Ausgangs 1", "Typ des Ausgangs 2",
                           "Typ des Ausgangs 3", "Typ des Ausgangs 4" },
        node.mOutputTypes, node.Localize);
      checkDeLocalizedList<StringValueObject>(
        new List<string> { "Formel 1", "Formel 2", "Formel 3", "Formel 4" },
        node.mTemplates, node.Localize);
      checkDeLocalizedList<IValueObject>(
        new List<string> { "Ausgang 1", "Ausgang 2", "Ausgang 3", "Ausgang 4" },
        node.mOutputs, node.Localize);

      // Rely on default output type for all outputs
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);
      Assert.AreEqual(PortTypes.Number, node.mOutputs[1].PortType.Name);
      Assert.AreEqual(PortTypes.Number, node.mOutputs[2].PortType.Name);
      Assert.AreEqual(PortTypes.Number, node.mOutputs[3].PortType.Name);

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 1, 3, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "n" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{ "rad", "deg", "x" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output states
      Assert.IsNotNull(node.mError);
      Assert.IsFalse(node.mError.HasValue);
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);         // should be double
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[2]);         // should be double
      Assert.IsFalse(node.mOutputs[2].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[3]);         // should be double
      Assert.IsFalse(node.mOutputs[3].HasValue);  // no output value yet

      // Set input values and re-check the output states
      node.mNumInputs[0].Value = Math.PI / 2; // rad
      node.mNumInputs[1].Value = 123.0;       // deg
      node.mIntInputs[0].Value = 2;
      node.mNumInputs[2].Value = Math.E;
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(90.0, (double)node.mOutputs[0].Value, 2e-13);
      Assert.IsNotNull(node.mOutputs[1]);         // should be double
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0.83867, (double)node.mOutputs[1].Value, 6e-7);
      Assert.IsNotNull(node.mOutputs[2]);         // should be double
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(2.0 * Math.PI, (double)node.mOutputs[2].Value, 2e-15);
      Assert.IsNotNull(node.mOutputs[3]);         // should be double
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(1.0, (double)node.mOutputs[3].Value, 2e-15);

      // Call a test that has only one expression and a different result type
      // to cover reducing the number of expressions and changing the output
      // type at the same time
      node.mTemplateCount.Value = 1;
      SimpleByte();
    }

    [Test]
    public void SimpleLighting()
    {
      // Use the default template; don't set the number of templates

      // Set and check the output type
      node.mOutputTypes[0].Value = PortTypes.Integer;
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);

      // Set a simple valid template that uses a few placeholders
      node.mTemplates[0].Value = "Light.RGBWToR((long){rgbw:F})";

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 1, 0);
      checkInputNames<BoolValueObject>(new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>(new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { "rgbw" },
                                                             node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { }, node.mStrInputs);

      // Check the output state
      Assert.IsNotNull(node.mOutputs[0]);         // should be bool
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet

      // Set input values and re-check the output
      node.mNumInputs[0].Value = 0xf713579bdf;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(-1, node.mOutputs[0].Value);

      // Change input values and re-check the output
      node.mNumInputs[0].Value = 0x8fdb97531;
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.AreEqual(0xfd, node.mOutputs[0].Value);
    }

    [Test]
    public void LightRGBFunctions()
    {
      // Use many valid expressions
      node.mTemplateCount.Value = 10;
      Assert.AreEqual(10, node.mTemplates.Count);
      node.mTemplates[0].Value = "Light.RGB((byte){r:I}, (byte){g:I}, (byte){b:I})";
      node.mTemplates[1].Value = "Light.RGBToR({rgb:I})";
      node.mTemplates[2].Value = "Light.RGBToG(_out1_)";
      node.mTemplates[3].Value = "Light.RGBToB({rgb})";
      node.mTemplates[4].Value = "Light.RGBW({r}, {g}, {b}, {w:I})";
      node.mTemplates[5].Value = "Light.RGBWToR((long){rgbw:N})";
      node.mTemplates[6].Value = "Light.RGBWToG((long){rgbw})";
      node.mTemplates[7].Value = "Light.RGBWToB((long)_out5_)";
      node.mTemplates[8].Value = "Light.RGBWToW((long){rgbw})";
      node.mTemplates[9].Value = "Light.RGBW({rgb})";

      // Set and check the output types
      node.mOutputTypes[0].Value = PortTypes.Integer;
      node.mOutputTypes[1].Value = PortTypes.Byte;
      node.mOutputTypes[2].Value = PortTypes.Byte;
      node.mOutputTypes[3].Value = PortTypes.Byte;
      node.mOutputTypes[4].Value = PortTypes.Number;
      node.mOutputTypes[5].Value = PortTypes.Integer;
      node.mOutputTypes[6].Value = PortTypes.Integer;
      node.mOutputTypes[7].Value = PortTypes.Integer;
      node.mOutputTypes[8].Value = PortTypes.Integer;
      node.mOutputTypes[9].Value = PortTypes.Number;
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[1].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[2].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[3].PortType.Name);
      Assert.AreEqual(PortTypes.Number, node.mOutputs[4].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[5].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[6].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[7].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[8].PortType.Name);
      Assert.AreEqual(PortTypes.Number, node.mOutputs[9].PortType.Name);

      // Check template and output names
      checkDeLocalizedList<StringValueObject>(
        new List<string> { "Formel 1", "Formel 2", "Formel 3", "Formel 4",
               "Formel 5", "Formel 6", "Formel 7", "Formel 8", "Formel 9", "Formel 10"},
        node.mTemplates, node.Localize);
      checkDeLocalizedList<IValueObject>(
        new List<string> { "Ausgang 1", "Ausgang 2", "Ausgang 3", "Ausgang 4",
               "Ausgang 5", "Ausgang 6", "Ausgang 7", "Ausgang 8", "Ausgang 9", "Ausgang 10"},
        node.mOutputs, node.Localize);

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 5, 1, 0);
      checkInputNames<BoolValueObject>  (new List<string>{ }, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "r", "g", "b", "rgb", "w" },
                                                              node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{ "rgbw" },
                                                              node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{ }, node.mStrInputs);

      // Check the output states
      Assert.IsNotNull(node.mError);
      Assert.IsFalse(node.mError.HasValue);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsFalse(node.mOutputs[2].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsFalse(node.mOutputs[3].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsFalse(node.mOutputs[4].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsFalse(node.mOutputs[5].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsFalse(node.mOutputs[6].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsFalse(node.mOutputs[7].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[8]);
      Assert.IsFalse(node.mOutputs[8].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[9]);
      Assert.IsFalse(node.mOutputs[9].HasValue);  // no output value yet

      // Set input values and re-check the output states
      node.mIntInputs[0].Value = 0x13;        // r
      node.mIntInputs[0].WasSet = false;      // assume this as a start value
      node.mIntInputs[1].Value = 0x79;        // g
      node.mIntInputs[2].Value = 0xdf;        // b
      node.mIntInputs[3].Value = 0x2468ac;    // rgb
      node.mIntInputs[4].Value = 0x5b;        // w
      node.mNumInputs[0].Value = 0xe19fcb52d; // rgbw
      node.mNumInputs[0].WasSet = false;      // assume this as a start value
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0x1379df, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0x24, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0x79, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(0xac, node.mOutputs[3].Value);
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsTrue(node.mOutputs[4].HasValue);   // now has an output value
      Assert.AreEqual(0xf1379df5b, node.mOutputs[4].Value);
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsTrue(node.mOutputs[5].HasValue);   // now has an output value
      Assert.AreEqual(0x19, node.mOutputs[5].Value);
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsTrue(node.mOutputs[6].HasValue);   // now has an output value
      Assert.AreEqual(0xfc, node.mOutputs[6].Value);
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsTrue(node.mOutputs[7].HasValue);   // now has an output value
      Assert.AreEqual(0xdf, node.mOutputs[7].Value);
      Assert.IsNotNull(node.mOutputs[8]);
      Assert.IsTrue(node.mOutputs[8].HasValue);   // now has an output value
      Assert.AreEqual(-1, node.mOutputs[8].Value);
      Assert.IsNotNull(node.mOutputs[9]);
      Assert.IsTrue(node.mOutputs[9].HasValue);   // now has an output value
      Assert.AreEqual(0xf00448824, node.mOutputs[9].Value);

      // Call a test that has only one expression and a different result type
      // to cover reducing the number of expressions and changing the output
      // type at the same time
      node.mTemplateCount.Value = 1;
      SimpleBool();
    }

    [Test]
    public void LightHSVFunctions()
    {
      // Use many valid expressions
      node.mTemplateCount.Value = 8;
      Assert.AreEqual(8, node.mTemplates.Count);
      node.mTemplates[0].Value = "Light.HSV((byte){h:I}, (byte){s:I}, (byte){v:I})";
      node.mTemplates[1].Value = "Light.HSVToH({hsv:I})";
      node.mTemplates[2].Value = "Light.HSVToS(_out1_)";
      node.mTemplates[3].Value = "Light.HSVToV({hsv})";
      node.mTemplates[4].Value = "Light.RGBToHSV((byte){r:I}, (byte){g:I}, (byte){b:I})";
      node.mTemplates[5].Value = "Light.RGBToHSV({rgb:I})";
      node.mTemplates[6].Value = "Light.HSVToRGB({hsv})";
      node.mTemplates[7].Value = "Light.HSVToRGB((byte){h},(byte)_out3_,(byte){v})";

      // Set and check the output types
      node.mOutputTypes[0].Value = PortTypes.Integer;
      node.mOutputTypes[1].Value = PortTypes.Byte;
      node.mOutputTypes[2].Value = PortTypes.Byte;
      node.mOutputTypes[3].Value = PortTypes.Byte;
      node.mOutputTypes[4].Value = PortTypes.Integer;
      node.mOutputTypes[5].Value = PortTypes.Integer;
      node.mOutputTypes[6].Value = PortTypes.Integer;
      node.mOutputTypes[7].Value = PortTypes.Integer;
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[0].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[1].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[2].PortType.Name);
      Assert.AreEqual(PortTypes.Byte, node.mOutputs[3].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[4].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[5].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[6].PortType.Name);
      Assert.AreEqual(PortTypes.Integer, node.mOutputs[7].PortType.Name);

      // Check template and output names
      checkDeLocalizedList<StringValueObject>(
        new List<string> { "Formel 1", "Formel 2", "Formel 3", "Formel 4",
               "Formel 5", "Formel 6", "Formel 7", "Formel 8" },
        node.mTemplates, node.Localize);
      checkDeLocalizedList<IValueObject>(
        new List<string> { "Ausgang 1", "Ausgang 2", "Ausgang 3", "Ausgang 4",
               "Ausgang 5", "Ausgang 6", "Ausgang 7", "Ausgang 8" },
        node.mOutputs, node.Localize);

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 8, 0, 0);
      checkInputNames<BoolValueObject>  (new List<string>{}, node.mBinInputs);
      checkInputNames<IntValueObject>   (new List<string>{ "h", "s", "v", "hsv",
                                                           "r", "g", "b", "rgb" },
                                                             node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string>{}, node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string>{}, node.mStrInputs);

      // Check the output states
      Assert.IsNotNull(node.mError);
      Assert.IsFalse(node.mError.HasValue);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsFalse(node.mOutputs[2].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsFalse(node.mOutputs[3].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsFalse(node.mOutputs[4].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsFalse(node.mOutputs[5].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsFalse(node.mOutputs[6].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsFalse(node.mOutputs[7].HasValue);  // no output value yet

      // Set input values and re-check the output states
      node.mIntInputs[0].Value = 0x76;        // h     118            166
      node.mIntInputs[1].Value = 0x13;        // s          19           0.07  
      node.mIntInputs[2].Value = 0x5f;        // v              95            0.37
      node.mIntInputs[3].Value = 0x2468ac;    // hsv    36 104 172    51 0.41 0,67
      node.mIntInputs[4].Value = 0x31;        // r
      node.mIntInputs[5].Value = 0x97;        // g
      node.mIntInputs[6].Value = 0xfd;        // b
      node.mIntInputs[7].Value = 0x42a80e;    // rgb
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0x76135f, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0x24, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0x13, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(0xac, node.mOutputs[3].Value);
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsTrue(node.mOutputs[4].HasValue);   // now has an output value
      Assert.AreEqual(0x95cefd, node.mOutputs[4].Value);
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsTrue(node.mOutputs[5].HasValue);   // now has an output value
      Assert.AreEqual(0x47eaa8, node.mOutputs[5].Value);
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsTrue(node.mOutputs[6].HasValue);   // now has an output value
      Assert.AreEqual(0xaca166, node.mOutputs[6].Value);
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsTrue(node.mOutputs[7].HasValue);   // now has an output value
      Assert.AreEqual(0x585f5d, node.mOutputs[7].Value);

      // Set new input values and re-check the output states
      node.mIntInputs[0].Value = 0x33;        // h      51            72
      node.mIntInputs[1].Value = 0x31;        // s          49           0.19  
      node.mIntInputs[2].Value = 0x2f;        // v              47            0.18
      node.mIntInputs[3].Value = 0x2400ac;    // hsv    36   0 172    51 0.00 0,67
      node.mIntInputs[4].Value = 0xfd;        // r
      node.mIntInputs[5].Value = 0x35;        // g
      node.mIntInputs[6].Value = 0x97;        // b
      node.mIntInputs[7].Value = 0x020001;    // rgb
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0x33312f, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0x24, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0x31, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(0xac, node.mOutputs[3].Value);
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsTrue(node.mOutputs[4].HasValue);   // now has an output value
      Assert.AreEqual(0xebcafd, node.mOutputs[4].Value);
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsTrue(node.mOutputs[5].HasValue);   // now has an output value
      Assert.AreEqual(0xebff02, node.mOutputs[5].Value);
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsTrue(node.mOutputs[6].HasValue);   // now has an output value
      Assert.AreEqual(0xacacac, node.mOutputs[6].Value);
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsTrue(node.mOutputs[7].HasValue);   // now has an output value
      Assert.AreEqual(0x2d2f26, node.mOutputs[7].Value);

      // Set new input values and re-check the output states
      node.mIntInputs[0].Value = 0xa8;        // h     168           236
      node.mIntInputs[1].Value = 0x63;        // s          99           0.39  
      node.mIntInputs[2].Value = 0xc1;        // v             193            0.76
      node.mIntInputs[3].Value = 0xb3c362;    // hsv   179 195  98   252 0.76 0.39
      node.mIntInputs[4].Value = 0xff;        // r
      node.mIntInputs[5].Value = 0xff;        // g
      node.mIntInputs[6].Value = 0xff;        // b
      node.mIntInputs[7].Value = 0x030405;    // rgb
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0xa863c1, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0xb3, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0x63, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(0x62, node.mOutputs[3].Value);
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsTrue(node.mOutputs[4].HasValue);   // now has an output value
      Assert.AreEqual(0x0000ff, node.mOutputs[4].Value);
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsTrue(node.mOutputs[5].HasValue);   // now has an output value
      Assert.AreEqual(0x956605, node.mOutputs[5].Value);
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsTrue(node.mOutputs[6].HasValue);   // now has an output value
      Assert.AreEqual(0x261762, node.mOutputs[6].Value);
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsTrue(node.mOutputs[7].HasValue);   // now has an output value
      Assert.AreEqual(0x767bc1, node.mOutputs[7].Value);

      // Set new input values and re-check the output states
      node.mIntInputs[0].Value = 0xd6;        // h     214           301
      node.mIntInputs[1].Value = 0x23;        // s          35           0.14  
      node.mIntInputs[2].Value = 0x11;        // v              17            0.07
      node.mIntInputs[3].Value = 0xffe39f;    // hsv   255 227 159   359 0.89 0.62
      node.mIntInputs[4].Value = 0x00;        // r
      node.mIntInputs[5].Value = 0x00;        // g
      node.mIntInputs[6].Value = 0x00;        // b
      node.mIntInputs[7].Value = 0xfffefd;    // rgb
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(0xd62311, node.mOutputs[0].Value);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      Assert.AreEqual(0xff, node.mOutputs[1].Value);
      Assert.IsNotNull(node.mOutputs[2]);
      Assert.IsTrue(node.mOutputs[2].HasValue);   // now has an output value
      Assert.AreEqual(0x23, node.mOutputs[2].Value);
      Assert.IsNotNull(node.mOutputs[3]);
      Assert.IsTrue(node.mOutputs[3].HasValue);   // now has an output value
      Assert.AreEqual(0x9f, node.mOutputs[3].Value);
      Assert.IsNotNull(node.mOutputs[4]);
      Assert.IsTrue(node.mOutputs[4].HasValue);   // now has an output value
      Assert.AreEqual(0x000000, node.mOutputs[4].Value);
      Assert.IsNotNull(node.mOutputs[5]);
      Assert.IsTrue(node.mOutputs[5].HasValue);   // now has an output value
      Assert.AreEqual(0x1502ff, node.mOutputs[5].Value);
      Assert.IsNotNull(node.mOutputs[6]);
      Assert.IsTrue(node.mOutputs[6].HasValue);   // now has an output value
      Assert.AreEqual(0x9f1115, node.mOutputs[6].Value);
      Assert.IsNotNull(node.mOutputs[7]);
      Assert.IsTrue(node.mOutputs[7].HasValue);   // now has an output value
      Assert.AreEqual(0x110f11, node.mOutputs[7].Value);
    }

    [Test]
    public void HLKFunctions()
    {
      // Start with a simple boolean expression
      SimpleBool();

      // Now change to two valid expressions
      node.mTemplateCount.Value = 2;
      Assert.AreEqual(2, node.mTemplates.Count);
      node.mTemplates[0].Value = "Hlk.AbsHumidity({T:N}, {rF:N})";
      node.mTemplates[1].Value = "Hlk.DewPoint({T:N}, {rF:N})";

      // Check template and output names
      checkDeLocalizedList<StringValueObject>(
        new List<string> { "Formel 1", "Formel 2" },
        node.mTemplates, node.Localize);
      checkDeLocalizedList<IValueObject>(
        new List<string> { "Ausgang 1", "Ausgang 2" },
        node.mOutputs, node.Localize);

      // Reset first output to be a number and check types
      node.mOutputTypes[0].Value = PortTypes.Number;
      Assert.AreEqual(PortTypes.Number, node.mOutputs[0].PortType.Name);
      // Rely on default output type for second output
      Assert.AreEqual(PortTypes.Number, node.mOutputs[1].PortType.Name);

      // Expect no validation error
      var result = node.Validate("de");
      Assert.IsFalse(result.HasError);

      // Check the resulting inputs
      checkInputCounts(0, 0, 2, 0);
      checkInputNames<BoolValueObject>(new List<string> { }, node.mBinInputs);
      checkInputNames<IntValueObject>(new List<string> { }, node.mIntInputs);
      checkInputNames<DoubleValueObject>(new List<string> { "T", "rF" },
                                                               node.mNumInputs);
      checkInputNames<StringValueObject>(new List<string> { }, node.mStrInputs);

      // Check the resulting outputs
      node.mOutputTypes[0].Value = PortTypes.Number;
      node.mOutputTypes[1].Value = PortTypes.Number;

      // Check the output states
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.IsFalse(node.mOutputs[0].HasValue);  // no output value yet
      Assert.IsNotNull(node.mOutputs[1]);         // should be double
      Assert.IsFalse(node.mOutputs[1].HasValue);  // no output value yet

      // Set input values and re-check the output states
      node.mNumInputs[0].Value = 30;  // °C
      node.mNumInputs[1].Value = 50;  // % rF
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);         // should be double
      Assert.IsTrue(node.mOutputs[0].HasValue);   // now has an output value
      Assert.AreEqual(15.2, (double)node.mOutputs[0].Value, 0.05);
      Assert.IsNotNull(node.mOutputs[1]);         // should be double
      Assert.IsTrue(node.mOutputs[1].HasValue);   // now has an output value
      // Assert.AreEqual(18.4, (double)node.mOutputs[1].Value, 0.1);

      // Change input values and re-check the output states
      node.mNumInputs[0].Value = 20;  // °C
      node.mNumInputs[1].Value = 60;  // % rF
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(10.4, (double)node.mOutputs[0].Value, 0.05);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);
      Assert.AreEqual(12.0, (double)node.mOutputs[1].Value, 0.1);

      // Change input values and re-check the output states
      node.mNumInputs[0].Value = 0;   // °C
      node.mNumInputs[1].Value = 40;  // % rF
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(1.9, (double)node.mOutputs[0].Value, 0.04);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);
      Assert.AreEqual(-12.0, (double)node.mOutputs[1].Value, 0.1);

      // Change input values and re-check the output states
      node.mNumInputs[0].Value = -10; // °C
      node.mNumInputs[1].Value = 50;  // % rF
      node.Execute();
      Assert.IsNotNull(node.mError);
      Assert.IsTrue(node.mError.HasValue);
      Assert.AreEqual("", node.mError.Value);
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsTrue(node.mOutputs[0].HasValue);
      Assert.AreEqual(1.2, (double)node.mOutputs[0].Value, 0.04);
      Assert.IsNotNull(node.mOutputs[1]);
      Assert.IsTrue(node.mOutputs[1].HasValue);
      Assert.AreEqual(-18.4, (double)node.mOutputs[1].Value, 0.1);
    }

    public class ExpressionCalculatorErrorTestCaseData
    {
      public static IEnumerable ErrorTestCases
      {
        get
        {
          // Own error messages (localized)
          yield return new TestCaseData("{:N}", "HasDefaultName",
                                        0, 0, 0, 0).
                               SetName("ErrorHasDefaultName");
          yield return new TestCaseData("{ :N}", "HasDefaultName",
                                        0, 0, 0, 0).
                               SetName("ErrorHasBlankName");
          yield return new TestCaseData("{x:N1}", "HasFormatOrMappings",
                                        0, 0, 0, 0).
                                SetName("ErrorHasFormat");
          yield return new TestCaseData("{x:N|0=null}", "HasFormatOrMappings",
                                        0, 0, 0, 0).
                                SetName("ErrorHasMappings");
          yield return new TestCaseData("{x 2:N}", "HasUnusableName",
                                        0, 0, 0, 0).
                                SetName("ErrorHasUnusableName");
          yield return new TestCaseData("{_x:N}", "PlaceholderNameInvalid",
                                        0, 0, 0, 0).
                                SetName("ErrorHasReservedName");
          yield return new TestCaseData("_out0_", "HasOutOfRangeRef",
                                        0, 0, 0, 0).
                                SetName("ErrorHasBelowRangeRef");
          yield return new TestCaseData("_out1_", "HasOutOfRangeRef",
                                        0, 0, 0, 0).
                                SetName("ErrorHasAboveRangeRef");
          yield return new TestCaseData("{2x:N}", "PlaceholderNameInvalid",
                                        0, 0, 0, 0).
                                SetName("ErrorPlaceholderNameInvalid");
          yield return new TestCaseData("{a:I} = {x:I}", "HasAssignment",
                                        0, 0, 0, 0).
                                SetName("ErrorAssignment1");
          yield return new TestCaseData("a={x:I}", "HasAssignment",
                                        0, 0, 0, 0).
                                SetName("ErrorAssignment2");
          // Errors causing exceptions in interpreter (not localized)
          yield return new TestCaseData("{x:N}^2", "",
                                        0, 0, 1, 0).
                                SetName("ExceptionSyntax");
          yield return new TestCaseData("{x:N}/0", "",
                                        0, 0, 1, 0).
                                SetName("ExceptionDivideByZero");
          yield return new TestCaseData("{x:B}/2", "",
                                        1, 0, 0, 0).
                                SetName("ExceptionBoolDivide");
        }
      }
    }
    [TestCaseSource(typeof(ExpressionCalculatorErrorTestCaseData),
                    "ErrorTestCases", Category = "ErrorTestCases")]
    public void ErrorTests(string template,
                           string expectedError,
                              int expNumBinInputs,
                              int expNumIntInputs,
                              int expNumNumInputs,
                              int expNumStrInputs)
    {
      // Execute the error test case
      node.mTemplates[0].Value = template;
      ValidationResult result = node.Validate("de");

      if ( expectedError.Length > 0 )
      { // Expect a validation error
        var messageDe = node.Localize("de", expectedError);
        // Some error messages have the offending token at the beginning.
        // We ignore this by comparing only the fixed part of the message.
        Assert.IsFalse(result.Message.StartsWith(" ")); // token mssing if true
        Assert.IsTrue(result.Message.EndsWith(messageDe));
        // Ensure that localized error messages exist 
        Assert.IsTrue(messageDe.Length > 40);
        var messageEn = node.Localize("en", expectedError);
        Assert.IsTrue(messageEn.Length > 40);
        Assert.IsTrue(expectedError.Length < 30);
      }

      // Check the outputs
      checkInputCounts(expNumBinInputs, expNumIntInputs,
                       expNumNumInputs, expNumStrInputs);
      assignDefaultInputValues();
      node.Execute();
      Assert.IsNotNull(node.mOutputs[0]);
      Assert.IsFalse(node.mOutputs[0].HasValue);
      Assert.IsNotNull(node.mError);

      if ( expectedError.Length <= 0 )
      { // Expect an execution error message at the error output 
        Assert.IsTrue(node.mError.HasValue);
        Assert.IsTrue(node.mError.Value.StartsWith(node.mTemplates[0].Name + ": "));
        Assert.IsTrue(node.mError.Value.Length > 15);
      }
    }
  }
}
