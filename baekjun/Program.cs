using System;
using System.Reflection.Metadata;

namespace Rekonban
{
    enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    class Program
    {
        static void Main()
        {
            // 기초 세팅
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Title = "Rekoban";
            Console.Clear();

            // 기호 상수 정의
            const int MAP_MIN_X = 0;
            const int MAP_MAX_X = 20;
            const int MAP_MIN_Y = 0;
            const int MAP_MAX_Y = 15;

            // 플레이어의 위치 좌표
            int playerX = 0;
            int playerY = 0;

            // 플레이어의 이동 방향
            Direction playerDirection = Direction.None; // 0:None / 1:Left / 2:Right / 3:Up / 4:Down

            // 플레이어가 민 박스의 데이터를 저장
            int pushedBoxIndex = 0;

            // 박스의 좌표
            int[] boxPositionsX = { 5, 8 };
            int[] boxPositionsY = { 5, 4 };

            // 벽의 좌표
            int[] wallPositionsX = { 7, 11 };
            int[] wallPositionsY = { 7, 5 };

            // 골의 좌표
            int[] goalPositionsX = { 10, 3 };
            int[] goalPositionsY = { 10, 6 };

            // 박스가 골 위에 있는지 판별할 수 있는 데이터 생성
            bool[] isBoxOnGoal = new bool[boxPositionsX.Length];

            // 오브젝트의 개수
            int wallCount = wallPositionsX.Length;
            int goalCount = goalPositionsX.Length;
            int boxCount = boxPositionsX.Length;

            while (true)
            {
                // ========================== Render ==========================
                // 이전 프레임 지우기
                Console.Clear();

                // 플레이어 그리기
                Console.SetCursorPosition(playerX, playerY);
                Console.Write("P");

                // 벽 그리기
                for (int i = 0; i < wallCount; ++i)
                {
                    Console.SetCursorPosition(wallPositionsX[i], wallPositionsY[i]);
                    Console.Write("W");
                }             

                // 골 그리기
                for (int i = 0; i < goalCount; ++i)
                {
                    Console.SetCursorPosition(goalPositionsX[i], goalPositionsY[i]);
                    Console.Write("G");
                }

                // 박스 그리기
                for (int i = 0; i < boxCount; ++i)
                {
                    Console.SetCursorPosition(boxPositionsX[i], boxPositionsY[i]);
                    if (isBoxOnGoal[i])
                    {
                        Console.Write("O");
                    } 
                    else
                    {
                        Console.Write("B");
                    }
                }

                // ========================== ProcessInput ==========================
                // 사용자로부터 입력을 받음
                ConsoleKeyInfo keyinfo = Console.ReadKey();
                ConsoleKey key = keyinfo.Key;


                // ========================== Update ==========================
                
                // 플레이어의 이동 처리
                if (key == ConsoleKey.LeftArrow)
                {
                    playerX = Math.Max(MAP_MIN_X, playerX - 1);
                    playerDirection = Direction.Left;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    playerX = Math.Min(playerX + 1, MAP_MAX_X);
                    playerDirection = Direction.Right;
                }

                if (key == ConsoleKey.UpArrow)
                {
                    playerY = Math.Max(MAP_MIN_Y, playerY - 1);
                    playerDirection = Direction.Up;
                }

                if (key == ConsoleKey.DownArrow) 
                {
                    playerY = Math.Min(playerY + 1, MAP_MAX_Y);
                    playerDirection = Direction.Down;
                }

                // 플레이어와 벽의 충돌 처리
                for (int i = 0; i < wallCount; ++i)
                {
                    if (playerX == wallPositionsX[i] && playerY == wallPositionsY[i])
                    {
                        switch (playerDirection)
                        {
                            case Direction.Left:
                                playerX = wallPositionsX[i] + 1;
                                break;

                            case Direction.Right:
                                playerX = wallPositionsX[i] - 1;
                                break;

                            case Direction.Up:
                                playerY = wallPositionsY[i] + 1;
                                break;

                            case Direction.Down:
                                playerY = wallPositionsY[i] - 1;
                                break;
                        }
                    }
                }      

                // 박스 이동 처리
                for (int i = 0; i < boxCount; ++i)
                {
                    if (playerX != boxPositionsX[i] || playerY != boxPositionsY[i])
                    {
                        continue;
                    }
                        switch (playerDirection)
                        {
                            case Direction.Left: //Left
                                boxPositionsX[i] = Math.Max(MAP_MIN_X, boxPositionsX[i] - 1);
                                playerX = boxPositionsX[i] + 1;
                                break;

                            case Direction.Right: // Right
                                boxPositionsX[i] = Math.Min(boxPositionsX[i] + 1, MAP_MAX_X);
                                playerX = boxPositionsX[i] - 1;
                                break;

                            case Direction.Up: // Up
                            boxPositionsY[i] = Math.Max(MAP_MIN_Y, boxPositionsY[i] - 1);
                                playerY = boxPositionsY[i] + 1;
                                break;

                            case Direction.Down: // Down
                            boxPositionsY[i] = Math.Min(boxPositionsY[i] + 1, MAP_MAX_Y);
                                playerY = boxPositionsY[i] - 1;
                                break;

                            default: //Error
                                Console.Clear();
                                Console.WriteLine($"[Error] 플레이어의 방향 설정이 잘못되었습니다. {playerDirection}");
                                Environment.Exit(1); // 프로그램 종료 
                                break;
                        }

                    // 어떤 박스를 밀었는지 저장
                    pushedBoxIndex = i;

                    break;
                }
                
                // 박스끼리의 충돌 처리
                for (int i = 0; i < boxCount; ++i)
                {
                    // 같은 박스라면 처리할 필요가 없음
                    if (pushedBoxIndex == i)
                    {
                        continue;
                    }

                    if (boxPositionsX[pushedBoxIndex] != boxPositionsX[i] && boxPositionsY[pushedBoxIndex] != boxPositionsY[i])
                    {
                        continue;
                    }

                    switch (playerDirection)
                    {
                        case Direction.Left:
                            boxPositionsX[pushedBoxIndex] = boxPositionsX[i] + 1;
                            playerX = boxPositionsX[pushedBoxIndex] + 1;
                            break;
                            
                        case Direction.Right:
                            boxPositionsX[pushedBoxIndex] = boxPositionsX[i] - 1;
                            playerX = boxPositionsX[pushedBoxIndex] - 1;
                            break;

                        case Direction.Up:
                            boxPositionsY[pushedBoxIndex] = boxPositionsY[i] + 1;
                            playerY = boxPositionsY[pushedBoxIndex] + 1;
                            break;

                        case Direction.Down:
                            boxPositionsY[pushedBoxIndex] = boxPositionsY[i] - 1;
                            playerY = boxPositionsY[pushedBoxIndex] - 1;
                            break;

                        default: // Error
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어의 방향 설정이 잘못되었습니다. {playerDirection}");
                            Environment.Exit(1); // 프로그램 종료

                            break;
                    }
                }

                // 박스와 벽의 충돌 처리
                for (int i = 0; i < wallCount; ++i)
                {
                    if (boxPositionsX[pushedBoxIndex] != wallPositionsX[i] || boxPositionsY[pushedBoxIndex] != wallPositionsY[i])
                    {
                        continue;
                    }

                        switch (playerDirection)
                        {
                            case Direction.Left:
                                boxPositionsX[pushedBoxIndex] = wallPositionsX[i] + 1;
                                playerX = boxPositionsX[pushedBoxIndex] + 1;
                                break;

                            case Direction.Right:
                                boxPositionsX[pushedBoxIndex] = wallPositionsX[i] - 1;
                                playerX = boxPositionsX[pushedBoxIndex] - 1;
                                break;

                            case Direction.Up:
                                boxPositionsY[pushedBoxIndex] = wallPositionsY[i] + 1;
                                playerY = boxPositionsY[pushedBoxIndex] + 1;
                                break;

                            case Direction.Down:
                                boxPositionsY[pushedBoxIndex] = wallPositionsY[i] - 1;
                                playerY = boxPositionsY[pushedBoxIndex] - 1;
                                break;

                        default: // Error
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어의 방향 설정이 잘못되었습니다. {playerDirection}");
                            Environment.Exit(1); // 프로그램 종료

                            break;
                        }

                    break;
                }

                // 모든 박스가 골 위에 올라왔는지 확인
                int boxOnGoalCount = 0;
                for (int boxId = 0; boxId < boxCount; ++boxId)
                {
                    // 현재 프레임의 박스 상태를 실시간으로 추적하기 위해 false로 바꾼다.
                    isBoxOnGoal[boxId] = false;
                    for (int goalId = 0; goalId < goalCount; ++goalId)
                    {
                        if (boxPositionsX[boxId] == goalPositionsX[goalId] && boxPositionsY[boxId] == goalPositionsY[goalId])
                        {
                            ++boxOnGoalCount;
                            isBoxOnGoal[boxId] = true;

                            break; // 이 박스에 대해 더 이상 체크할 필요가 없음
                        }
                    }
                }
                if (boxOnGoalCount == goalCount)
                {
                    break;
                }
            }

            Console.Clear();
            Console.WriteLine("축하합니다. 게임을 클리어 하셨습니다.");

            // 게임이 끝났으니 콘솔 세팅을 원상복구
            Console.ResetColor();
        }
    }
}