using System;
using System.Collections.Generic;
using UnityEngine;
using CocoDoogy.GameFlow.InGame.Command.Content;
using CocoDoogy.Test;

namespace CocoDoogy.GameFlow.InGame.Command
{
    /// <summary>
    /// 모든 Command는 CommandManager를 통해 진행해야 함
    /// </summary>
    public static partial class CommandManager
    {
        public static Stack<CommandBase> Executed { get; private set; } = new();
        public static Stack<CommandBase> Undid { get; private set; } = new();


        public static void Clear()
        {
            Executed.Clear();
            Undid.Clear();
        }


        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 게임플레이에 영향을 주는 Command 작업 수행
        /// </summary>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static CommandBase ExecuteCommand(CommandType type, object param)
        {
            CommandBase result = null;
            try
            {
                result = type switch
                {
                    CommandType.Move => new MoveCommand(param),
                    CommandType.Trigger => new TriggerCommand(param),
                    
                    CommandType.Slide => new SlideCommand(param),
                    CommandType.Teleport => new TeleportCommand(param),
                    CommandType.Sail => new SailCommand(param),
                    
                    CommandType.Deploy => new DeployCommand(param),
                    CommandType.Refill => new RefillCommand(param),
                    CommandType.SandCount => new SandCountCommand(param),
                    CommandType.Weather => new WeatherCommand(param),
                    CommandType.Gimmick => new GimmickCommand(param),
                    CommandType.Increase => new IncreaseCommand(param),
                    CommandType.DeckReset => new DeckResetCommand(param),
                    
                    CommandType.MaxUp => new MaxUpItemCommand(param),
                    CommandType.Recover => new RecoverItemCommand(param),
                    CommandType.Undo => new UndoItemCommand(param),
                    _ => null
                };

                if (result != null)
                {
                    result.Execute();
                    Executed.Push(result);
                    Undid.Clear();
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogError(e.Message);
            }

            return result;
        }

        #region ◇ Undo & Redo ◇
        /// <summary>
        /// 유저가 바로 전까지 조작한 부분까지 자동 Undo
        /// </summary>
        public static void UndoCommandAuto()
        {
            Debug.Log($"Executed: {Executed.Count}");
            while (Executed.Count > 0)
            {
                CommandBase command = UndoCommand();
                if (command.IsUserCommand)
                {
                    ApplySystemCommands();
                    return;
                }
            }
        }
        /// <summary>
        /// 유저가 마지막으로 조작한 부분까지 자동 Redo
        /// </summary>
        public static void RedoCommandAuto()
        {
            Debug.Log($"Undid: {Undid.Count}");
            while (Undid.Count > 0)
            {
                CommandBase command = RedoCommand();
                if (command.IsUserCommand)
                {
                    ApplySystemCommands();
                    return;
                }
            }
        }
        /// <summary>
        /// Redo, Undo 후 적용돼야 하는 자동 생성 Command를 적용
        /// </summary>
        private static void ApplySystemCommands()
        {
            while (Undid.Count > 0)
            {
                CommandBase command = Undid.Pop();
                Undid.Push(command);

                if (command.IsUserCommand) return;
                RedoCommand();
            }
        }

        private static CommandBase UndoCommand() {
            if (!Executed.TryPop(out CommandBase command)) return null;
            
            command.Undo();
            Undid.Push(command);
            TileOutlineDrawer.Draw();
            
            return command;
        }
        private static CommandBase RedoCommand() {
            if (!Undid.TryPop(out CommandBase command)) return null;
            
            command.Execute();
            Executed.Push(command);
            TileOutlineDrawer.Draw();

            return command;
        }
        #endregion
    }
}