using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;



//include GLM library
using GlmNet;

using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Tao.Platform.Windows;

namespace Graphics
{
    class Renderer
    {
        Shader sh;

        //buffer id declarations
        uint nucleus1BufferID;
        uint nucleus2BufferID;
        uint nucleus3BufferID;
        uint nucleus4BufferID;
        uint nucleus5BufferID;

        uint electron1BufferID;
        uint electron2BufferID;
        uint electron3BufferID;
        uint electron4BufferID;
        uint electron5BufferID;

        //model matrix declarations
        mat4 nucleus1ModelMatrix;
        mat4 nucleus2ModelMatrix;
        mat4 nucleus3ModelMatrix;
        mat4 nucleus4ModelMatrix;
        mat4 nucleus5ModelMatrix;

        mat4 electron1ModelMatrix;
        mat4 electron2ModelMatrix;
        mat4 electron3ModelMatrix;
        mat4 electron4ModelMatrix;
        mat4 electron5ModelMatrix;


        mat4 ViewMatrix;
        mat4 ProjectionMatrix;

        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        //function to draw nucleus. takes center coordinates and number protons and neutrons you want to draw
        public float[] Nucleus(float h, float k, float increment, float num)
        {
            double x, y, z, R=0, G=0, B = 0, x1, y1;
            x1 = h;
            y1 = k;

            List<float> coordinates = new List<float>();

            for (float i = 0; i < num; i+=1)
            {

                if (i == 0)
                {
                    h += 1f;
                    R = 1;
                    G = 0;
                    B = 0;
                }
                else if (i == 1)
                {
                    h -= 2f;
                    R = 0;
                    G = 0;
                    B = 1;
                }
                else if (i == 2)
                {
                    h = (float)x1;
                    k += 1f;
                    R = 1;
                    G = 0;
                    B = 0;
                }
                else if (i == 3)
                {
                    k -= 2f;
                    R = 0;
                    G = 0;
                    B = 1;
                }

                int horizontalLines = 50, verticalLines = 50;

                for (int m = 0; m < horizontalLines; m++)
                {
                    for (int n = 0; n < verticalLines - 1; n++)
                    {
                        x = h + Math.Sin(Math.PI * m / horizontalLines) * Math.Cos(2 * Math.PI * n / verticalLines);
                        y = k + Math.Sin(Math.PI * m / horizontalLines) * Math.Sin(2 * Math.PI * n / verticalLines);
                        z = Math.Cos(Math.PI * m / horizontalLines);

                        coordinates.Add((float)x);
                        coordinates.Add((float)y);
                        coordinates.Add((float)z);
                        coordinates.Add((float)R);
                        coordinates.Add((float)G);
                        coordinates.Add((float)B);
                    }
                }
            }

            float[] array = coordinates.ToArray();

            return array;
        }

        //fucntion to draw electrons. takes center coordinates and draws 2 electrons offset at the x axis
        public float[] Electron(float h, float k, float increment)
        {
            double x, y, z, R=1, G=1, B=1;

            List<float> coordinates = new List<float>();

            for (float i = 0; i < 2; i += 1)
            {

                if (i % 2 == 0)
                {
                    h = h - 6f;
                }
                else
                {
                    h = h + 12f;
                }

                int horizontalLines = 50, verticalLines = 50;

                for (int m = 0; m < horizontalLines; m++)
                {
                    for (int n = 0; n < verticalLines - 1; n++)
                    {
                        x = h + Math.Sin(Math.PI * m / horizontalLines) * Math.Cos(2 * Math.PI * n / verticalLines);
                        y = k + Math.Sin(Math.PI * m / horizontalLines) * Math.Sin(2 * Math.PI * n / verticalLines);
                        z = Math.Cos(Math.PI * m / horizontalLines);

                        coordinates.Add((float)x);
                        coordinates.Add((float)y);
                        coordinates.Add((float)z);
                        coordinates.Add((float)R);
                        coordinates.Add((float)G);
                        coordinates.Add((float)B);
                    }
                }
            }

            float[] array = coordinates.ToArray();

            return array;
        }

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0f, 1);

            //points array initilizations

            float[] nucleus1 = Nucleus(0f, 0f, 1, 4);
            float[] nucleus2 = Nucleus(10f, 10f, 1, 4);
            float[] nucleus3 = Nucleus(-10f, 10f, 1, 4);
            float[] nucleus4 = Nucleus(10f, -10f, 1, 4);
            float[] nucleus5 = Nucleus(-10f, -10f, 1, 4);

            float[] electron1 = Electron(0f, 0f, 1);
            float[] electron2 = Electron(10f, 10f, 1);
            float[] electron3 = Electron(-10f, 10f, 1);
            float[] electron4 = Electron(10f, -10f, 1);
            float[] electron5 = Electron(-10f, -10f, 1);


            //buffer id initilizations

            nucleus1BufferID = GPU.GenerateBuffer(nucleus1);
            nucleus2BufferID = GPU.GenerateBuffer(nucleus2);
            nucleus3BufferID = GPU.GenerateBuffer(nucleus3);
            nucleus4BufferID = GPU.GenerateBuffer(nucleus4);
            nucleus5BufferID = GPU.GenerateBuffer(nucleus5);

            electron1BufferID = GPU.GenerateBuffer(electron1);
            electron2BufferID = GPU.GenerateBuffer(electron2);
            electron3BufferID = GPU.GenerateBuffer(electron3);
            electron4BufferID = GPU.GenerateBuffer(electron4);
            electron5BufferID = GPU.GenerateBuffer(electron5);


            // Model Matrix Initialization
            nucleus1ModelMatrix = new mat4(1);
            nucleus2ModelMatrix = new mat4(1);
            nucleus3ModelMatrix = new mat4(1);
            nucleus4ModelMatrix = new mat4(1);
            nucleus5ModelMatrix = new mat4(1);


            electron1ModelMatrix = new mat4(1);
            electron2ModelMatrix = new mat4(1);
            electron3ModelMatrix = new mat4(1);
            electron4ModelMatrix = new mat4(1);
            electron5ModelMatrix = new mat4(1);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(30, 30, 30),
                        new vec3(0, 0, 0),
                        new vec3(0, 1, 0)
                );

            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            sh.UseShader();

            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus1ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 2, Gl.GL_FALSE, nucleus2ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 3, Gl.GL_FALSE, nucleus3ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 4, Gl.GL_FALSE, nucleus4ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 5, Gl.GL_FALSE, nucleus5ModelMatrix.to_array());

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 6, Gl.GL_FALSE, electron1ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 7, Gl.GL_FALSE, electron2ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 8, Gl.GL_FALSE, electron3ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 9, Gl.GL_FALSE, electron4ModelMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 10, Gl.GL_FALSE, electron5ModelMatrix.to_array());

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);


            //electron1
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, electron1BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, electron1ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //electron2
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, electron2BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, electron2ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //electron3
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, electron3BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, electron3ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //electron4
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, electron4BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, electron4ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //electron5
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, electron5BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, electron5ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //nucleus2
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, nucleus2BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus2ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 2, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 3, 2450);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //nucleus3
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, nucleus3BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus3ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 2, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 3, 2450);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //nucleus4
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, nucleus4BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus4ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 2, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 3, 2450);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //nucleus5
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, nucleus5BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus5ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 2, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 3, 2450);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            //nucleus1
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, nucleus1BufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, nucleus1ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 0, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 1, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 2, 2450);
            Gl.glDrawArrays(Gl.GL_POLYGON, 2450 * 3, 2450);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);


        }

        //translation coordinates for moving
        public float translationX=0, translationY=0, translationZ=0;

        const float rotationSpeed = 0.01f;


        float nucleusRotationAngle = 0;
        float electronRotationAngle = 0;

        vec3 nucleus2Center;

        vec3 nucleus3Center;

        vec3 nucleus4Center;

        vec3 nucleus5Center;

        public void Update()
        {
            //define nucleus centers
            nucleus2Center = new vec3(-10f, -10f, 0);

            nucleus3Center = new vec3(10f, -10f, 0);

            nucleus4Center = new vec3(-10f, 10f, 0);

            nucleus5Center = new vec3(10f, 10f, 0);

            nucleusRotationAngle += rotationSpeed * 2;
            electronRotationAngle += rotationSpeed * 5;

            List<mat4> electron1transformations = new List<mat4>();
            electron1transformations.Add(glm.rotate(electronRotationAngle, new vec3(0, 0, 1)));
            electron1transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));
            electron1ModelMatrix = MathHelper.MultiplyMatrices(electron1transformations);

            List<mat4> electron2transformations = new List<mat4>();
            electron2transformations.Add(glm.translate(new mat4(1), nucleus2Center));
            electron2transformations.Add(glm.rotate(electronRotationAngle, new vec3(0, 0, 1)));
            electron2transformations.Add(glm.translate(new mat4(1), nucleus2Center * -1));
            electron2transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            electron2ModelMatrix = MathHelper.MultiplyMatrices(electron2transformations);

            List<mat4> electron3transformations = new List<mat4>();
            electron3transformations.Add(glm.translate(new mat4(1), nucleus3Center));
            electron3transformations.Add(glm.rotate(electronRotationAngle, new vec3(0, 0, 1)));
            electron3transformations.Add(glm.translate(new mat4(1), nucleus3Center * -1));
            electron3transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            electron3ModelMatrix = MathHelper.MultiplyMatrices(electron3transformations);

            List<mat4> electron4transformations = new List<mat4>();
            electron4transformations.Add(glm.translate(new mat4(1), nucleus4Center));
            electron4transformations.Add(glm.rotate(electronRotationAngle, new vec3(0, 0, 1)));
            electron4transformations.Add(glm.translate(new mat4(1), nucleus4Center * -1));
            electron4transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            electron4ModelMatrix = MathHelper.MultiplyMatrices(electron4transformations);

            List<mat4> electron5transformations = new List<mat4>();
            electron5transformations.Add(glm.translate(new mat4(1), nucleus5Center));
            electron5transformations.Add(glm.rotate(electronRotationAngle, new vec3(0, 0, 1)));
            electron5transformations.Add(glm.translate(new mat4(1), nucleus5Center * -1));
            electron5transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            electron5ModelMatrix = MathHelper.MultiplyMatrices(electron5transformations);


            List<mat4> nucleus1transformations = new List<mat4>();
            nucleus1transformations.Add(glm.scale(new mat4(1), new vec3(2, 2f, 2)));
            nucleus1transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus1transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));
            nucleus1ModelMatrix = MathHelper.MultiplyMatrices(nucleus1transformations);

            List<mat4> nucleus2transformations = new List<mat4>();
            nucleus2transformations.Add(glm.translate(new mat4(1), nucleus2Center));
            nucleus2transformations.Add(glm.scale(new mat4(1), new vec3(2, 2f, 2)));
            nucleus2transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus2transformations.Add(glm.translate(new mat4(1), nucleus2Center * -1));
            nucleus2transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus2ModelMatrix = MathHelper.MultiplyMatrices(nucleus2transformations);

            List<mat4> nucleus3transformations = new List<mat4>();
            nucleus3transformations.Add(glm.translate(new mat4(1), nucleus3Center));
            nucleus3transformations.Add(glm.scale(new mat4(1), new vec3(2, 2f, 2)));
            nucleus3transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus3transformations.Add(glm.translate(new mat4(1), nucleus3Center * -1));
            nucleus3transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus3ModelMatrix = MathHelper.MultiplyMatrices(nucleus3transformations);

            List<mat4> nucleus4transformations = new List<mat4>();
            nucleus4transformations.Add(glm.translate(new mat4(1), nucleus4Center));
            nucleus4transformations.Add(glm.scale(new mat4(1), new vec3(2, 2f, 2)));
            nucleus4transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus4transformations.Add(glm.translate(new mat4(1), nucleus4Center * -1));
            nucleus4transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus4ModelMatrix = MathHelper.MultiplyMatrices(nucleus4transformations);

            List<mat4> nucleus5transformations = new List<mat4>();
            nucleus5transformations.Add(glm.translate(new mat4(1), nucleus5Center));
            nucleus5transformations.Add(glm.scale(new mat4(1), new vec3(2, 2f, 2)));
            nucleus5transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus5transformations.Add(glm.translate(new mat4(1), nucleus5Center * -1));
            nucleus5transformations.Add(glm.rotate(nucleusRotationAngle, new vec3(0, 0, 1)));
            nucleus5ModelMatrix = MathHelper.MultiplyMatrices(nucleus5transformations);
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
