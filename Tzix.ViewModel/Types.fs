﻿namespace Tzix.ViewModel

open Tzix.Model

[<AutoOpen>]
module Types =
  type FileNodeViewModel =
    {
      FileNodeId        : Id
      ShortName         : string
    }