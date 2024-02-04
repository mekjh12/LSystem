using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    class LoaderLSystem
    {
        /// <summary>
        /// 줄기와 잎을 분리하여 3d 모델로 읽어온다.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="delta"></param>
        /// <param name="branchLength"></param>
        /// <returns></returns>
        public static (RawModel3d, RawModel3d) Load3dWithLeaf(string word, float delta, float branchLength = 1.0f)
        {
            List<float> branchList = new List<float>();
            List<float> leafList = new List<float>();

            Quaternion pose = Quaternion.Identity;
            Vertex3f pos = Vertex3f.Zero;

            float r = branchLength;

            // 문자열을 순회하면서 경로를 만든다.
            Stack<Quaternion> stack = new Stack<Quaternion>();
            Stack<Vertex3f> posStack = new Stack<Vertex3f>();

            int isLeafDrawed = -1;
            Vertex3f orginalPos = Vertex3f.Zero;

            int idx = 0;

            while (idx < word.Length)
            {
                if (word.Length == 0) break;
                char c = word[idx];

                Vertex3f forward = ((Matrix4x4f)pose).ForwardVector();
                Vertex3f up = ((Matrix4x4f)pose).UpVector();
                Vertex3f left = ((Matrix4x4f)pose).LeftVector();

                if (c == 'F' || c == 'A')
                {
                    Vertex3f start = pos;
                    Vertex3f end = start + forward * r;

                    branchList.Add(start.x);
                    branchList.Add(start.y);
                    branchList.Add(start.z);
                    branchList.Add(end.x);
                    branchList.Add(end.y);
                    branchList.Add(end.z);

                    pos = end;
                }
                else if (c == 'f')
                {
                    isLeafDrawed++;

                    Vertex3f start = pos;
                    Vertex3f end = start + forward * r * 0.8f;

                    if (isLeafDrawed != 0)
                    {
                        if (orginalPos != end)
                        {
                            leafList.Add(orginalPos.x);
                            leafList.Add(orginalPos.y);
                            leafList.Add(orginalPos.z);
                            leafList.Add(start.x);
                            leafList.Add(start.y);
                            leafList.Add(start.z);
                            leafList.Add(end.x);
                            leafList.Add(end.y);
                            leafList.Add(end.z);
                        }
                    }

                    pos = end;
                }
                else if (c == '+')
                {
                    pose = up.Rotate(delta).Concatenate(pose);
                }
                else if (c == '-')
                {
                    pose = up.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '|')
                {
                    pose = up.Rotate(180).Concatenate(pose);
                }
                else if (c == '&')
                {
                    pose = left.Rotate(delta).Concatenate(pose);
                }
                else if (c == '^')
                {
                    pose = left.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '\\')
                {
                    pose = forward.Rotate(delta).Concatenate(pose);
                }
                else if (c == '/')
                {
                    pose = forward.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '[')
                {
                    stack.Push(pose);
                    posStack.Push(pos);
                }
                else if (c == ']')
                {
                    pose = stack.Pop();
                    pos = posStack.Pop();
                }
                else if (c == '{')
                {
                    orginalPos = pos;
                    isLeafDrawed++;
                }
                else if (c == '}')
                {
                    isLeafDrawed = -1;
                }
                idx++;
            }

            // raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo = StoreDataInAttributeList(0, 3, branchList.ToArray());
            Gl.BindVertexArray(0);
            RawModel3d branchRawModel = new RawModel3d(vao, branchList.ToArray());


            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            vbo = StoreDataInAttributeList(0, 3, leafList.ToArray());
            Gl.BindVertexArray(0);
            RawModel3d leafRawModel = new RawModel3d(vao, leafList.ToArray());

            return (branchRawModel, leafRawModel);
        }

        /// <summary>
        /// 문장을 가지고 3d모델로 읽어온다. 스택에서 쿼터니온과 위치를 저장한다.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="delta"></param>
        /// <param name="branchLength"></param>
        /// <returns></returns>
        public static RawModel3d Load3d(string word, float delta, float branchLength = 1.0f)
        {
            List<float> list = new List<float>();

            Quaternion pose = Quaternion.Identity;
            Vertex3f pos = Vertex3f.Zero;

            float r = branchLength;

            // 문자열을 순회하면서 경로를 만든다.
            Stack<Quaternion> stack = new Stack<Quaternion>();
            Stack<Vertex3f> posStack = new Stack<Vertex3f>();

            int idx = 0;

            while (idx < word.Length)
            {
                if (word.Length == 0) break;
                char c = word[idx];

                Vertex3f forward = ((Matrix4x4f)pose).ForwardVector();
                Vertex3f up = ((Matrix4x4f)pose).UpVector();
                Vertex3f left = ((Matrix4x4f)pose).LeftVector();

                if (c == 'F' || c == 'A')
                {
                    Vertex3f start = pos;
                    Vertex3f end = start + forward * r;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    pos = end;
                }
                else if (c == 'f')
                {
                    Vertex3f start = pos;
                    Vertex3f end = start + forward * r;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    pos = end;
                }
                else if (c == '+')
                {
                    pose = up.Rotate(delta).Concatenate(pose);
                }
                else if (c == '-')
                {
                    pose = up.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '|')
                {
                    pose = up.Rotate(180).Concatenate(pose);
                }
                else if (c == '&')
                {
                    pose = left.Rotate(delta).Concatenate(pose);
                }
                else if (c == '^')
                {
                    pose = left.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '\\')
                {
                    pose = forward.Rotate(delta).Concatenate(pose);
                }
                else if (c == '/')
                {
                    pose = forward.Rotate(-delta).Concatenate(pose);
                }
                else if (c == '[')
                {
                    stack.Push(pose);
                    posStack.Push(pos);
                }
                else if (c == ']')
                {
                    pose = stack.Pop();
                    pos = posStack.Pop();
                }

                idx++;
            }

            // raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, list.ToArray());
            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, list.ToArray());

            return rawModel;
        }

        /// <summary>
        /// 3d에서 2d로 모델을 읽어온다.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="delta"></param>
        /// <param name="branchLength"></param>
        /// <returns></returns>
        public static RawModel3d Load2d(string word, float delta, float branchLength = 1.0f)
        {
            List<float> list = new List<float>();

            Vertex4f pose = new Vertex4f(0, 0, 90);
            float r = branchLength;

            // draw mode
            Stack<Vertex4f> stack = new Stack<Vertex4f>();

            Vertex3f end = new Vertex3f();
            Vertex3f start = new Vertex3f();

            while (true)
            {
                if (word.Length == 0) break;
                char c = word[0];
                word = word.Substring(1);

                if (c == 'F' || c == 'X')
                {
                    start.x = pose.x;
                    start.y = pose.y;
                    float deg = pose.z;
                    float rad = deg * 3.141502f / 180.0f;
                    end.x = (float)(r * Math.Cos(rad)) + start.x;
                    end.y = (float)(r * Math.Sin(rad)) + start.y;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(0);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(0);

                    pose.x = end.x;
                    pose.y = end.y;
                }
                else if (c == '+')
                {
                    pose.z += delta;
                }
                else if (c == '-')
                {
                    pose.z -= delta;
                }
                else if (c == '[')
                {
                    stack.Push(new Vertex3f(pose.x, pose.y, pose.z));
                }
                else if (c == ']')
                {
                    pose = stack.Pop();
                }
            }

            // raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, list.ToArray());
            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, list.ToArray());

            return rawModel;
        }

        public static unsafe uint StoreDataInAttributeList(uint attributeNumber, int coordinateSize, float[] data, BufferUsage usage = BufferUsage.StaticDraw)
        {
            // VBO 생성
            uint vboID = Gl.GenBuffer();

            // VBO의 데이터를 CPU로부터 GPU에 복사할 때 사용하는 BindBuffer를 다음과 같이 사용
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(data.Length * sizeof(float)), data, usage);

            // 이전에 BindVertexArray한 VAO에 현재 Bind된 VBO를 attributeNumber 슬롯에 설정
            Gl.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribType.Float, false, 0, IntPtr.Zero);
            //Gl.VertexArrayVertexBuffer(glVertexArrayVertexBuffer, vboID, )

            // GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboID;
        }
    }
}
