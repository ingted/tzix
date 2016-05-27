﻿namespace Tzix.ViewModel

open System.IO
open Tzix.Model
open Basis.Core
open Dyxi.Util.Wpf

type MainWindowViewModel() as this =
  inherit ViewModel.Base()

  let dictFile = FileInfo(@"tzix.json")

  let mutable _dict =
    try
      File.ReadAllText(dictFile.FullName) |> Dict.ofJson
    with | _ ->
#if DEBUG
      Dict.createForDebug ()
#else
      Dict.empty
#endif

  let _foundListViewModel = FoundListViewModel()

  let mutable _searchText = ""

  let searchIncrementally () =
    let items =
      if _searchText |> Str.isNullOrWhiteSpace
      then Seq.empty
      else
        _dict
        |> Dict.findInfix _searchText
        |> Seq.map (FileNodeViewModel.ofFileNode _dict)
    do _foundListViewModel.Items <- items |> Seq.toObservableCollection

  let _setSearchText v =
    _searchText <- v
    this.RaisePropertyChanged("SearchText")
    searchIncrementally ()

  let _commitCommand =
    Command.create (fun _ -> true) (fun _ ->
      _foundListViewModel.SelectFirstIfNoSelection()
      _foundListViewModel.TrySelectedItem() |> Option.iter (fun item ->
        let node      = _dict |> Dict.findNode item.FileNodeId
        _dict <- _dict |> Dict.execute node
        _setSearchText ""
        ))
    |> fst

  member this.SearchText
    with get () = _searchText
    and  set v  = _setSearchText v

  member this.FoundList = _foundListViewModel

  member this.CommitCommand = _commitCommand

  member this.Save() =
    try
      File.WriteAllText(dictFile.FullName, _dict |> Dict.toJson)
    with | _ ->
      ()
